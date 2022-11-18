using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.Reflection;

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
            var qry = from u in _context.TcUsers.AsNoTracking().Include("Directorate").Where(x => x.UserId == username && !string.IsNullOrWhiteSpace(x.EmployeePin))
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
        public async Task<TcUser> GetTcDirUserInfo(string username)
        {
            return await _context.TcUsers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == username);
        }
        public bool SetRecommender(string username, string recommId)
        {
            var ManagerSettings = _context.UserManagers.Find(username);
            var recommUser = _context.TcUsers.AsNoTracking().FirstOrDefault(x => x.UserId == recommId); ;
            if (ManagerSettings != null && recommUser != null)
            {
                ManagerSettings.RecommenderId = recommUser.UserId;
                ManagerSettings.RecommenderSurname = recommUser.SurName;
                ManagerSettings.RecommenderGivenName = recommUser.GivenName;
            }

            return _context.SaveChanges() > 0;
        }
        public async Task<List<DirectReportsModel>> GetMyEmployees(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var directReports = from m in _context.UserManagers.Where(x => x.ManagerId == username)
                                    join u in _context.TcUsers.AsNoTracking() on m.UserId equals u.UserId
                                    select new DirectReportsModel() { Email = null, FullName = u.SurName + ", " + u.GivenName, UserName = u.UserId };
                return await directReports.ToListAsync();
            }
            else
            {
                return new List<DirectReportsModel>();
            }

        }
        public async Task<bool> DeleteFromMyEmployees(string emolpoyee_id, string username)
        {
            var employee = await _context.UserManagers.FirstOrDefaultAsync(x => x.ManagerId == username && x.UserId == emolpoyee_id);
            if (employee != null)
            {
                employee.ManagerId = null;
                employee.ManagerGivenName = null;
                employee.ManagerSurname = null;
                employee.LastUpdateByUserId = username;

            }
            // reutrn agreement if status =2, 3, 4 (submitted, recommended, or approved)
            var agmts =  _context.Agreements.Where(x => x.TcUserId == emolpoyee_id && !x.ArchivedInd);
            foreach (var agmt in agmts)
            {
                if (agmt.EndDate< DateTime.Today || agmt.StartDate> DateTime.Today)//expaired or not started
                {
                    agmt.ArchivedInd = true;
                }
                agmt.StatusCode = "1";
                agmt.RecommenderId = null;
                agmt.ApproverId = null;
            }
            return _context.SaveChanges() >0;
        }
        public List<DirectReportsModel> GetManagers(string prefix)
        {
            List<DirectReportsModel> directReports;
            prefix = Regex.Replace(prefix, @"\s+", " ");
            var names = prefix.ToLower().Split(' ');
            string query = @"select * from VW_WAA_MANAGER where (LOWER(CONVERT(USER_GIVEN_NAME, 'US7ASCII')) LIKE '%{0}%' or LOWER(USER_GIVEN_NAME) LIKE '%{1}%')
                            AND (LOWER(CONVERT(USER_FAMILY_NAME, 'US7ASCII')) like '%{3}%' or LOWER(USER_FAMILY_NAME) LIKE '%{3}%')";
            query = string.Format(query, names[0], names[0], names.Length > 1 ? names[1] : "", names.Length > 1 ? names[1] : "");

            var mgr = _context.Managers.FromSqlRaw(query);

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
            string fullName = Regex.Replace(managerFullName, @"\s+", " ");//remove xtra spaces
            if (user != null)
            {
                user.ManagerId = managerId;
                user.ManagerSurname = fullName.Split(' ')[1];
                user.ManagerGivenName = fullName.Split(' ')[0];
                user.LastUpdateByUserId = userId;
            }
            else
            {
                _context.UserManagers.Add(new UserManager() { 
                    UserId = userId, 
                    ManagerId= managerId,
                    ManagerSurname = fullName.Split(' ')[1], 
                    ManagerGivenName = fullName.Split(' ')[0],
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
        
        public async Task<List<TMXMember>> GetAllTmxUser()
        {
            return await _context.TMXMembers.Include(x => x.TcUser).ToListAsync();

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
        public TcUser GetRecommendBy(int agreementId, string recommToId)
        {
            TcUser usr = null;
            string recommenderId = _context.Agreements.FirstOrDefault(x => x.AgreementId == agreementId && x.RecommenderId == recommToId)?.ApproverId;
            var recommenders = _context.FTRRecommenders.FirstOrDefault(x => x.AgreementId == agreementId);
            if (recommenders!=null)
            {
                var tt = recommenders.GetType().GetProperties();
                string[] rs = { recommenders.RecommerLevel1, recommenders.RecommerLevel2, recommenders.RecommerLevel3, recommenders.RecommerLevel4 };
                int index = Array.IndexOf(rs, recommToId);
                if (index > 0)
                {
                    recommenderId = rs[index - 1];
                } 
            }
          
            if (!string.IsNullOrEmpty(recommenderId))
            {
                usr = _context.TcUsers.FirstOrDefault(x => x.UserId == recommenderId);
            }
            return usr;
        }
        public bool IsTMX(string userId)
        {
            return _context.TMXMembers.Any(x=>x.Tc_user_id == userId && x.EffectiveEndDate == null);
        }
        public bool AddTMXUser(string TmxUserId, string updatedById)
        {
            bool userExist = _context.TMXMembers.Any(x => x.Tc_user_id == TmxUserId);
            if (!userExist)
            {
                _context.TMXMembers.Add(new TMXMember()
                {
                    Tc_user_id = TmxUserId,
                    EffectiveStartDate = DateTime.Today,
                    LastUpdateByUserId = updatedById,
                    LastUpdateDate = DateTime.Now
                });
            }
            return _context.SaveChanges() > 0;
        }
        public bool UpdateTMXUser(string TmxUserId, string sdate, string edate, string updatedById)
        {
            if (string.IsNullOrEmpty(sdate)) return false;

            var tmxUser = _context.TMXMembers.FirstOrDefault(x => x.Tc_user_id == TmxUserId);
            if (tmxUser!=null)
            {
                tmxUser.EffectiveStartDate = DateTime.Parse(sdate);
                if (string.IsNullOrEmpty(edate))
                {
                    tmxUser.EffectiveEndDate = null;
                }
                else
                {
                    tmxUser.EffectiveEndDate = DateTime.Parse(edate);
                }
                tmxUser.LastUpdateByUserId = updatedById;
                tmxUser.LastUpdateDate = DateTime.Now;
            }
            return _context.SaveChanges() > 0;
        }
        public bool DeleteTMXUser(string suerUserId)
        {
            var user = _context.TMXMembers.Find(suerUserId);
            _context.TMXMembers.Remove(user);
            return _context.SaveChanges() > 0;

        }
    }
}
