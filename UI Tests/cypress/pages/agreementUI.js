import { Select } from '../helpers/Utils';
import semantic from '../helpers/semantic.js';
import addAddressModalUI from '../pages/addAddressModalUI.js';

const agreementUI = {
    StartIWAButton: Select('agreement-start-btn'),
    ReopenIWAButton: Select('reopen-agreement-btn'),
    ContinueNextSectionIWAButton: Select('agreement-continue-btn'),
    GoBackASectionIWAButton: Select('agreement-prev-btn'),
    StatusLabel: Select('status-label'),
    MissingADInfoMsg: Select('missing-ad-msg'),

    Section1UserDetails: Select('userDetails'),
    Section1SupervisorUpdateLink: Select('supervisor-update-link'),

    Section2ObligationsCheckbox: Select('obligations-check'),
    Section2ConditionsCheckbox: Select('conditions-check'),

    Section3WorkTypeRemoteRadioBtn: Select('worktype-check-1'),
    Section3WorkTypePSPCOfficeRadioBtn: Select('worktype-check-2'),
    Section3WorkTypeHybridRadioBtn: Select('worktype-check-3'),

    Section4DaysInOfficeDropdown: Select('daysinoffice-select'),
    Section4NeighbourhoodDropdown: Select('neighbourhood-select'),
    Section4AddHomeAddressButton: Select('add-address-btn'), 
    Section4AddressListItem: Select('address-list-item'),   
    Section4DeleteAddressButton: Select('delete-address-btn'),

    SummaryWorkType: Select('summary-worktype'),
    SummaryDateRange: Select('summary-daterange'),
    SummaryRemoteWorkplace: Select('summary-remoteworkplace'),
    SummaryDaysInOffice: Select('summary-daysinoffice'),
    SummaryNeighbourhood: Select('summary-neighbourhood'),

    SectionStartAgreement: Select('section-startagreement'),
    SectionVerifyYourInfo: Select('section-verifyyourinfo'),
    SectionTerms: Select('section-terms'),
    SectionWorkTypeSelection: Select('section-worktypeselection'),
    SectionWorkTypeDetails: Select('section-worktypedetails'),
    SectionSummary: Select('section-summary'),

    AgreementWorkflowError: Select('agreement-form-error-msg'),
    WorkplaceRequiredError: Select('workplace-required-error'),  
    ErrorListItem: Select('error-list-item'),


    // actions the user can take
    Start: () => {
        cy.get(agreementUI.StartIWAButton).click();
    },

    Reopen: () => {
        cy.get(agreementUI.ReopenIWAButton).click();
    },

    Continue: () => {
        cy.get(agreementUI.ContinueNextSectionIWAButton).click();
    },

    Back: () => {
        cy.get(agreementUI.GoBackASectionIWAButton).click();
    },

    UpdateSupervisor: () => {
        cy.get(agreementUI.Section1SupervisorUpdateLink).click();
    },
    
    CompleteSection1UserInfo: () => {
        agreementUI.Continue();
    },

    CompleteSection2TermsAndConditions: () => {
        cy.get(agreementUI.Section2ObligationsCheckbox).parent().click();
        cy.get(agreementUI.Section2ConditionsCheckbox).parent().click();        
        agreementUI.Continue();
    },

    CompleteSection3ChooseRemoteWorkType: () => {
        cy.get(agreementUI.Section3WorkTypeRemoteRadioBtn).parent().click();
        agreementUI.Continue();
    },

    CompleteSection3ChoosePSPCOfficeWorkType: () => {
        cy.get(agreementUI.Section3WorkTypePSPCOfficeRadioBtn).parent().click();
        agreementUI.Continue();
    },

    CompleteSection3ChooseHybridWorkType: () => {
        cy.get(agreementUI.Section3WorkTypeHybridRadioBtn).parent().click();
        agreementUI.Continue();
    },

    CompleteSection4ForPSPCWorkType: (workplaceDetails) => {
        semantic.SelectFromDropdown(agreementUI.Section4NeighbourhoodDropdown, workplaceDetails.City);
        //agreementUI.AddNeighbourhood(workplaceDetails.City);
        agreementUI.Continue();
    },

    CompleteSection4ForHybridWorkType: (workplaceDetails) => {
        agreementUI.AddAddress(workplaceDetails.Address);   
        semantic.SelectFromDropdown(agreementUI.Section4DaysInOfficeDropdown, workplaceDetails.DaysInOffice);
        agreementUI.AddNeighbourhood(workplaceDetails.City);
        agreementUI.Continue();
    },
   
    CompleteSection4ForRemoteWorkType: (workplaceDetails) => {
        agreementUI.AddAddress(workplaceDetails.Address); 
        agreementUI.Continue();
    },

    AddAddress: (address) => {
        cy.get(agreementUI.Section4AddHomeAddressButton).click();
        addAddressModalUI.AddAddress(address);
    },

    DeleteAddress: (address) => {
        cy.get(agreementUI.Section4DeleteAddressButton + '[data-street="' + address.streetAddress + '"]').click();
    },

    AddNeighbourhood: (city) => {
        cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('input.search').click().type(city, { force: true });
        cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('.menu').children('.item.selected').contains(city).click();
    },

    DeleteNeighbourhood: (city) => {
        cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('a.label').contains(city).find('.delete.icon').click();
    },    

    VerifyFormData: (dataSet) => {
        cy.get(agreementUI.WorkType).should('contain', dataSet.verification.worktype);
    },

    VerifyStatus: (status) => {
        cy.get(agreementUI.StatusLabel).should('contain', status);
    },
};

export default {
    ...agreementUI
};