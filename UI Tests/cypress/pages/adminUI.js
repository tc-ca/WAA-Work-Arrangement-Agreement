import { Select } from '../helpers/Utils.js';

const adminUI = {
    WorkLocationsButton: Select('link-neighbourhoods-admin'),
    AgreementsByBranchReportButton: Select('link-bybranch-report'),
    AgreementsByLocationReportButton: Select('link-bylocation-report'),

    GoToWorkLocations: () => {
        cy.get(adminUI.WorkLocationsButton).click();
    },

    DownloadAgreementsByBranchReport: () => {
        cy.get(adminUI.AgreementsByBranchReportButton).click();
    },

    DownloadAgreementsByLocationReport: () => {
        cy.get(adminUI.AgreementsByLocationReportButton).click();
    },
}

export default {
    ...adminUI
}