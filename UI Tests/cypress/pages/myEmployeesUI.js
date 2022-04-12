import { Select } from '../helpers/Utils';

const myEmployeesUI = {
    // pieces of the ui a user will interact with
    EmployeeLink: Select('edit-link'),
    RecommendedLink: Select('recommended-link'),
    EmployeeRow: Select('employee-row'),
    RecommendedRow: Select('recommended-row'),
    Status: Select('status-label'),

    ViewEmployeeIWA: (username) => {
        myEmployeesUI.GetEmployeeRow(username).find(myEmployeesUI.EmployeeLink).click();
    },   

    ViewRecommendedIWA: (username) => {
        myEmployeesUI.GetRecommendedRow(username).find(myEmployeesUI.RecommendedLink).click();
    }, 

    VerifyEmployeeStatus: (username, status) => {
        myEmployeesUI.GetEmployeeRow(username).get(myEmployeesUI.Status).contains(status);
    },

    VerifyRecommendedStatus: (username, status) => {
        myEmployeesUI.GetRecommendedRow(username).get(myEmployeesUI.Status).contains(status);
    },

    GetEmployeeRow: (username) => {
        return cy.get(myEmployeesUI.EmployeeRow + '[data-username="' + username + '"]');
    },

    GetRecommendedRow: (username) => {
        return cy.get(myEmployeesUI.RecommendedRow + '[data-username="' + username + '"]');
    }
};

export default {
    ...myEmployeesUI
};