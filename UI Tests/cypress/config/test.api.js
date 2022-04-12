import config from '../config/test.config.js';
import testData from '../config/test.data.js';

const api = {   

    CreateReportingStructure: () => {
        cy.request(config.URL + '/api/cypress/setmanager?empUsername=' + config.users.EmployeeUser.Username + '&mgrUsername=' + config.users.ManagerUser.Username).then((xhr) => {
            assert.isTrue(xhr.body);
        });
        cy.request(config.URL + '/api/cypress/setmanager?empUsername=' + config.users.ManagerUser.Username + '&mgrUsername=' + config.users.DirectorUser.Username).then((xhr) => {
            assert.isTrue(xhr.body);
        });
    },

    CleanupReportingStructure: () => {
        api.CleanTestUserData(config.users.EmployeeUser.Username);
        api.CleanTestUserData(config.users.ManagerUser.Username);
        api.CleanTestUserData(config.users.DirectorUser.Username);
    },

    SetManagerInPSPC: (username, mgrUsername) => {
        cy.request(config.URL + '/api/cypress/setmanager?empUsername=' + username + '&mgrUsername=' + mgrUsername).then((xhr) => {
            assert.isTrue(xhr.body);
        });
    },

    CleanTestUserData: (username) => {
        cy.request(config.URL + '/api/cypress/cleantestuserdata?username=' + username).then((xhr) => {
            assert.isTrue(xhr.body);
        });
    },

    CleanTestNeighbourhoods: () => {
        cy.request(config.URL + '/api/cypress/cleantestneighbourhoods').then((xhr) => {
            assert.isTrue(xhr.body);
        });
    },

    WaitForADUpdate: () => {
        cy.log("Waiting for Active Directory updates...")
        cy.wait(5000);
    },

    CreateAgreement: (user) => {
        config.Login(user.Username, user.Password);
        cy.visit(config.URL + '/en/Agreement');
    
        cy.get('body').then($body => {
            var rvt = Cypress.$('input[name=__RequestVerificationToken]').val();
            
            cy.request({
                method: 'POST',
                url: config.URL + '/en/agreement',
                form: true,
                body: {
                    'MyAgreement.UserName': user.Username,
                    'MyAgreement.FullName': user.DisplayName,
                    'MyAgreement:AgreementID': 0,
                    'MyAgreement:StatusID': 0,
                    'MyAgreement:ObligationsConfirmed': true,
                    'MyAgreement:ConditionsConfirmed': true,
                    'MyAgreement.WorkTypeID': testData.Agreement.WorkTypeID,
                    'MyAgreement:DaysInOffice': testData.Agreement.DaysInOffice,
                    'SelectedNeighborhoods': testData.Agreement.SelectedNeighborhoods,
                    '__RequestVerificationToken': rvt
                }
            });
        });
    }

}

export default api;
