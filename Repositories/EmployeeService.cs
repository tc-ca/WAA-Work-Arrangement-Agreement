using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Repositories
{
    public class EmployeeService
    {       
        private readonly DomainContext _context;

        public EmployeeService( DomainContext context)
        {
            _context = context;
        }

        public async Task<TcUser> GetTcUserInfo(string username)
        {
            TcUser user = null;
            var qry = from u in _context.TcUsers.AsNoTracking().Include("Directorate").Include("Branch").Where(x => x.UserId == username && !string.IsNullOrWhiteSpace(x.EmployeePin))
                      join m in _context.UserManagers.AsNoTracking() on u.UserId equals m.UserId into um
                      from r in um.DefaultIfEmpty()
                      select new { u, r };
            var rec = await qry.FirstOrDefaultAsync();
            if (rec != null)
            {
                user= rec.u;
                user.Manager = rec.r;
                user.EmergencyContact = await _context.EmployeeContact.AsNoTracking().Where(x => x.AddressType == "5" && x.Pri == user.EmployeePin)
                    .Select(u => new EmergencyContact() { ContactName = u.FullName, Telephone = u.PhoneNumber }).FirstOrDefaultAsync();
            }

            return user;
        }

        public async Task<List<DirectReportsModel>> GetMyEmployees(string username)
        {
            var directReports = from m in _context.UserManagers.Where(x=>x.ManagerId==username)
                    join u in _context.TcUsers.AsNoTracking() on m.UserId equals u.UserId
                    select  new DirectReportsModel() { Email = null, FullName = u.SurName + "," + u.GivenName, UserName = u.UserId };
            //var directReports =await _context.TcUsers.Where(x => x.Manager.ManagerId == username)
            //    .Select(e => new DirectReportsModel() { Email=null, FullName= e.SurName + "," + e.GivenName, UserName = e.UserId }).ToListAsync();

            return await directReports.ToListAsync();
        }
        public List<DirectReportsModel> GetManagers(string prefix)
        {
            List<DirectReportsModel> directReports;
            prefix = Regex.Replace(prefix, @"\s+", " ");
            var names = prefix.ToLower().Split(' ');
            var mgr = _context.Managers.Where(x => x.ManagerGivenName.ToLower().Contains(names[0]));
            if (names.Length > 1)
            {
                mgr = _context.Managers.Where(x => x.ManagerGivenName.ToLower().Contains(names[0]) && x.ManagerSurname.ToLower().Contains(names[1]));
            }
            directReports = mgr.Join(_context.TcUsers, m => m.UserId, u => u.UserId,
                               (m, u) => new DirectReportsModel() { Email = null, FullName = m.ManagerName, UserName = m.UserId })
                               .Take(10).ToList();
            return directReports;
        }

        public List<TcRegion> GetTcRegions()
        {
            return _context.Regions.AsNoTracking()
                .OrderBy(x => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "en" ? x.English : x.French)
                .ToList();
        }
        public List<WorkSite> GetWorksites(string region)
        {
            if (string.IsNullOrEmpty(region))
                return _context.WorkSites.AsNoTracking().ToList();
            else
                return _context.WorkSites.AsNoTracking().Where(x=>x.RegionCode==region).ToList();
        }

        public TcRegion RegionById(string region)
        {
            return _context.Regions.AsNoTracking().Where(x => x.Id == region).FirstOrDefault();
        }
        public bool UpdateManager(string userId, string managerId,string managerFullName)
        {
            var user = _context.UserManagers.FirstOrDefault(x=>x.UserId == userId);
            if (user != null)
            {
                user.ManagerId = managerId;
                user.ManagerSurname = managerFullName.Split(' ')[1];
                user.ManagerGivenName = managerFullName.Split(' ')[0];
                user.LastUpdateByUserId = userId;
            }
            else
            {
                _context.UserManagers.Add(new UserManager() { 
                    UserId = userId, 
                    ManagerId= managerId,
                    ManagerSurname = managerFullName.Split(' ')[1], 
                    ManagerGivenName = managerFullName.Split(' ')[0],
                    LastUpdateByUserId = userId
                });
                    
            }
            return _context.SaveChanges() > 0;
        }
        public bool AddSuperUser(string suerUserId, string updatedById)
        {
            bool userExist = _context.SuperUsers.Any(x=>x.UserId== suerUserId);
            if (!userExist)
            {
                _context.SuperUsers.Add(new SuperUser()
                {
                    UserId = suerUserId,
                    LastUpdateByUserId = updatedById

                });
            }
            return _context.SaveChanges()> 0;
             
        }
        public bool DeleteSuperUser(string suerUserId)
        {
            var user = _context.SuperUsers.Find(suerUserId);
            _context.SuperUsers.Remove(user);
            return _context.SaveChanges() > 0;

        }
        public async Task< List<SuperUser>> GetAllSuperUser()
        {
            return await _context.SuperUsers.Include(x => x.TcUser).ToListAsync();
            
        }
        public async Task<bool> IsSuperUser(string UserId)
        {
            return await _context.SuperUsers.AnyAsync(x=>x.UserId== UserId);

        }
        public async Task<List<TcUser>> SearchTcUserInfo(string prefix)
        {
            prefix = Regex.Replace(prefix, @"\s+", " ");
            var names = prefix.ToLower().Split(' ');
            var user = new List<TcUser>();
            if (names.Length == 1)
            {
                user = await _context.TcUsers.AsNoTracking().Where(x => x.GivenName.ToLower().Contains(names[0])).ToListAsync();
            } else if (names.Length > 1)
            {
                user = await _context.TcUsers.AsNoTracking().Where(x => x.GivenName.ToLower().Contains(names[0]) && x.SurName.ToLower().Contains(names[1])).ToListAsync();
            }

            return user;
        }

    }
}
