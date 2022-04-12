import { Select } from '../helpers/Utils';

const LayoutUI = {
    // pieces of the ui a user will interact with
    MyAgreementLink: Select('navMyAgreement'),
    MyEmployeesLink: Select('navMyEmployees'),
    AdministrationLink: Select('navAdministration'),
    SwitchLanguageButton: Select('languageButton'),
    
    HelpButton: Select('helpButton'),
    HelpModal: Select('helpModal'),
    CloseHelpButton: Select('managerHelp_close'),

    GoToMyAgreement: () => {
        cy.get(LayoutUI.MyAgreementLink).click();
    },

    GoToMyEmployees: () => {
        cy.get(LayoutUI.MyEmployeesLink).click();
    },

    GoToAdministration: () => {
        cy.get(LayoutUI.AdministrationLink).click();
    },

    SwitchLanguage: () => {
        cy.get(LayoutUI.SwitchLanguageButton).click();
    },

    OpenHelpScreen: () => {
        cy.get(LayoutUI.HelpButton).click();
    },

    CloseHelpScreen: () => {
        cy.get(LayoutUI.CloseHelpButton).click();
    }
};

export default {
    ...LayoutUI
};