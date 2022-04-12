import { Select } from '../helpers/Utils';

const editAgreementUI = {
    ActionRequestButton: Select('action-link'),
    BackButton: Select('back-btn'),
    SummaryDetails: Select('summary-details'),

    ActionRequest: () => {
        cy.get(editAgreementUI.ActionRequestButton).click();
    },

    Back: () => {
        cy.get(editAgreementUI.BackButton).click();
    }
};

export default {
    ...editAgreementUI
};