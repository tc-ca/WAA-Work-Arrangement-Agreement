import layoutUI from '../pages/layoutUI.js';
import config from '../config/test.config.js';
import api from '../config/test.api.js'

context('Navigation and Layout', () => {

    before(() => {
        api.CreateReportingStructure();
    });

    after(() => {
        api.CleanupReportingStructure();
    });


    it('Ensure proper nav links are present and working for an employee with no direct reports.', () => {
        config.LoginAsEmployeeUser();
        cy.visit(config.URL + '/en/Agreement/Index');

        cy.get(layoutUI.MyAgreementLink).should('exist'); 
        layoutUI.GoToMyAgreement();
        cy.url().should('contain', '/en/agreement');

        cy.get(layoutUI.MyEmployeesLink).should('not.exist');        
    });


    it('Ensure proper nav links are present and working for a manager.', () => {
        config.LoginAsManagerUser(); 
        cy.visit(config.URL + '/en/Agreement/Index');

        cy.get(layoutUI.MyAgreementLink).should('exist'); 
        layoutUI.GoToMyAgreement();
        cy.url().should('contain', '/en/agreement');

        cy.get(layoutUI.MyEmployeesLink).should('exist');
        layoutUI.GoToMyEmployees();
        cy.url().should('contain', '/en/myemployees');        
    });


    it('Ensure admin link is absent for a non-admin user.', () => {
        config.LoginAsEmployeeUser();
        cy.visit(config.URL + '/en/Agreement/Index');

        cy.get(layoutUI.AdministrationLink).should('not.exist');        
    });


    it('Ensure admin link is present and working for an admin user.', () => {
        config.LoginAsAdminUser();
        cy.visit(config.URL + '/en/Agreement/Index');

        cy.get(layoutUI.AdministrationLink).should('exist');
        layoutUI.GoToAdministration();
        cy.url().should('contain', '/en/admin');
    });


    it('Ensure language can be switched from English to French and vice versa.', () => {
        config.LoginAsEmployeeUser();
        cy.visit(config.URL + '/en/Agreement/Index');

        layoutUI.SwitchLanguage();
        cy.url().should('contain', '/fr/entente');

        layoutUI.SwitchLanguage();
        cy.url().should('contain', '/en/agreement');
    });


    it('Ensure the help modal can be opened and closed.', () => {
        config.LoginAsEmployeeUser();
        cy.visit(config.URL + '/en/Agreement/Index');

        layoutUI.OpenHelpScreen();
        cy.get(layoutUI.HelpModal).should('be.visible');

        layoutUI.CloseHelpScreen();
        cy.get(layoutUI.HelpModal).should('not.be.visible');
    });

});