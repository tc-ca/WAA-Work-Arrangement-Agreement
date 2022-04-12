import myEmployeesUI from '../pages/myEmployeesUI.js';
import editAgreementUI from '../pages/editAgreementUI.js';
import actionRequestModalUI from '../pages/actionRequestModalUI.js';
import config from '../config/test.config.js';
import api from '../config/test.api.js';
import layoutUI from '../pages/layoutUI.js';

context('Manager features', () => {
    before(() => {
        api.CreateReportingStructure();        
    });

    beforeEach(() => {
        api.CleanTestUserData(config.users.EmployeeUser.Username);        
    })

    after(() => {
        api.CleanupReportingStructure();
    });


    it('Ensure My Employees link DOES NOT display for users that do not have employees', () => {
        config.LoginAsEmployeeUser();
        cy.visit(config.URL + '/en/Agreement/Index');

        cy.get(layoutUI.MyEmployeesLink).should('not.exist');
    });


    it('Ensure My Employees link displays for managers and at least one employee is listed on their dashboard.', () => {
        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/Agreement/Index');

        cy.get(layoutUI.MyEmployeesLink).should('exist');
        layoutUI.GoToMyEmployees();

        cy.get(myEmployeesUI.EmployeeRow).should('have.length.at.least', 1);
    });


    it('Ensure manager can indicate they are level 4.', () => {
        api.CreateAgreement(config.users.EmployeeUser);

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();

        actionRequestModalUI.ChooseLevel4ManagerResponse('yes');
        cy.get(actionRequestModalUI.Level4ManagerYesButton).should('have.value', 1);
    });


    it('Ensure a level 4 manager can leave a comment and approve an IWA.', () => {
        api.CreateAgreement(config.users.EmployeeUser);

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.ApproveRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'Approved');
    });


    it('Ensure a level 4 manager can leave a comment and deny an IWA.', () => {
        api.CreateAgreement(config.users.EmployeeUser);

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.DenyRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'Denied');
    });


    it('Ensure a non-level 4 manager can leave a comment and recommend to their supervisor.', () => {
        api.CreateAgreement(config.users.EmployeeUser);

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.RecommendRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'Recommended');

        config.LoginAsDirectorUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.GetRecommendedRow(config.users.EmployeeUser.Username).should('exist');
    });


    it('Ensure a recommended IWA can be approved.', () => {
        api.CreateAgreement(config.users.EmployeeUser);

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.RecommendRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'Recommended');

        config.LoginAsDirectorUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewRecommendedIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.ApproveRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyRecommendedStatus(config.users.EmployeeUser.Username, 'Approved');
    });


    it('Ensure a recommended IWA can be denied.', () => {
        api.CreateAgreement(config.users.EmployeeUser);

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.RecommendRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'Recommended');

        config.LoginAsDirectorUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewRecommendedIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.DenyRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyRecommendedStatus(config.users.EmployeeUser.Username, 'Denied');
    });


    it('Ensure manager can view an approved IWA.', () => {
        api.CreateAgreement(config.users.EmployeeUser);

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.ApproveRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'Approved');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        cy.get(editAgreementUI.SummaryDetails).should('exist');
    });


    it('Ensure recommendee can view an approved IWA.', () => {
        api.CreateAgreement(config.users.EmployeeUser);

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.RecommendRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'Recommended');

        config.LoginAsDirectorUser();
        cy.visit(config.URL + '/en/MyEmployees');

        myEmployeesUI.ViewRecommendedIWA(config.users.EmployeeUser.Username);
        editAgreementUI.ActionRequest();
        actionRequestModalUI.ApproveRequest();

        cy.get(actionRequestModalUI.Modal).should('not.exist');
        myEmployeesUI.VerifyRecommendedStatus(config.users.EmployeeUser.Username, 'Approved');

        myEmployeesUI.ViewRecommendedIWA(config.users.EmployeeUser.Username);
        cy.get(editAgreementUI.SummaryDetails).should('exist');
    });

});
