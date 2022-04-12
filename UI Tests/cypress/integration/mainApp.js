import semantic from '../helpers/semantic.js';
import myEmployeesUI from '../pages/myEmployeesUI.js';
import agreementUI from '../pages/agreementUI';
import supervisorUpdateModalUI from '../pages/supervisorUpdateModalUI.js';
import config from '../config/test.config.js';
import testData from '../config/test.data.js';
import api from '../config/test.api.js';

context('Main App', () => {

    before(() => {
        api.CreateReportingStructure();
    });


    beforeEach(() => {
        api.CleanTestUserData(config.users.EmployeeUser.Username);
        config.LoginAsEmployeeUser();
        cy.visit(config.URL + '/en/Agreement/Index');
    });


    after(() => {
        api.CleanupReportingStructure();
    });
    

    it('Start Section: Ensure user with no IWA sees the Start message on their first visit.', () => {
        cy.get(agreementUI.StartIWAButton).should('be.visible');
    });


    it('Start Section: Ensure IWA status is initially "Not Started".', () => {
        agreementUI.VerifyStatus('Not started');
    });


    it('Verify Info Section: Ensure status is set to "In progress" once a new IWA is started.', () => {
        agreementUI.Start();
        agreementUI.VerifyStatus('In progress');
    });


    it('Verify Info Section: Ensure employee information is displayed.', () => {
        agreementUI.Start();
        cy.get(agreementUI.Section1UserDetails).should('be.visible');
    });


    it('Verify Info Section: Ensure "Update your supervisor" link opens modal window with PSPCD link.', () => {
        agreementUI.Start();
        agreementUI.UpdateSupervisor();
        cy.get(supervisorUpdateModalUI.Modal).should('be.visible');
        cy.get(supervisorUpdateModalUI.PSPCDLink).should('be.visible');
    });


    it('Verify Info Section: Ensure PSPCD link has target set to "_blank" so it opens in a new tab.', () => {
        agreementUI.Start();
        agreementUI.UpdateSupervisor();
        cy.get(supervisorUpdateModalUI.Modal).should('be.visible');
        cy.get(supervisorUpdateModalUI.PSPCDLink).invoke('attr', 'target').should('contain', '_blank');
    });


    it('Verify Info Section: Ensure "Update your supervisor" modal can be closed.', () => {
        agreementUI.Start();
        agreementUI.UpdateSupervisor();
        cy.get(supervisorUpdateModalUI.Modal).should('be.visible');
        supervisorUpdateModalUI.Close();
        cy.get(supervisorUpdateModalUI.Modal).should('not.be.visible');
    });


    it('Verify Info Section: Ensure user can proceed to next page.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        cy.get(agreementUI.SectionTerms).should('be.visible');
    });


    it('Terms Section: Ensure that both confirmation boxes must be checked.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        
        cy.log('No boxes checked...');
        agreementUI.Continue();
        cy.get(agreementUI.AgreementWorkflowError).should('be.visible');
        cy.get(agreementUI.ErrorListItem).should('have.length', 2);

        cy.log('One box checked...');
        semantic.ToggleCheckbox(agreementUI.Section2ObligationsCheckbox);
        agreementUI.Continue();
        cy.get(agreementUI.AgreementWorkflowError).should('be.visible');
        cy.get(agreementUI.ErrorListItem).should('have.length', 1);

        cy.log('Other box checked...');
        semantic.ToggleCheckbox(agreementUI.Section2ObligationsCheckbox);
        semantic.ToggleCheckbox(agreementUI.Section2ConditionsCheckbox);
        agreementUI.Continue();
        cy.get(agreementUI.AgreementWorkflowError).should('be.visible');
        cy.get(agreementUI.ErrorListItem).should('have.length', 1);
    });


    it('Terms Section: Ensure user can proceed to next page.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        cy.get(agreementUI.SectionWorkTypeSelection).should('be.visible');
    });


    it('Work Type Selection Section: Ensure user can\'t proceed until a preferred arrangement is selected.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.Continue();

        cy.get(agreementUI.AgreementWorkflowError).should('be.visible');
        cy.get(agreementUI.ErrorListItem).should('have.length', 1);

        agreementUI.CompleteSection3ChooseHybridWorkType();
        cy.get(agreementUI.SectionWorkTypeDetails).should('be.visible');
    });


    it('Remote Type Details Section: Ensure only non-pspc workplace fields are showing.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();

        cy.get(agreementUI.Section4AddHomeAddressButton).should('be.visible');
        cy.get(agreementUI.Section4DaysInOfficeDropdown).parent().should('not.be.visible');
        cy.get(agreementUI.Section4NeighbourhoodDropdown).parent().should('not.be.visible');
    });


    it('Remote Type Details Section: Ensure workplaces can be added and removed.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();

        agreementUI.AddAddress(testData.WorkplaceDetails.Address);
        cy.get(agreementUI.Section4AddressListItem).should('have.length', 1);

        agreementUI.DeleteAddress(testData.WorkplaceDetails.Address);
        cy.get(agreementUI.Section4AddressListItem).should('not.exist');
    });


    it('Remote Type Details Section: Ensure user can\'t proceed without at least one workplace listed.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();

        cy.get(agreementUI.Section4AddressListItem).should('not.exist');
        agreementUI.Continue();

        // This assertion should really be checking whether the error message is focused (not just visible), but I couldn't get that to work.
        cy.get(agreementUI.WorkplaceRequiredError).should('be.visible');

        agreementUI.AddAddress(testData.WorkplaceDetails.Address);
        cy.get(agreementUI.Section4AddressListItem).should('have.length', 1);
        cy.get(agreementUI.WorkplaceRequiredError).should('not.be.visible');

        agreementUI.Continue();
        cy.get(agreementUI.SectionSummary).should('be.visible');
    });


    it('PSPC Office Details Section: Ensure only city field is showing.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();

        cy.get(agreementUI.Section4AddHomeAddressButton).should('not.be.visible');
        cy.get(agreementUI.Section4DaysInOfficeDropdown).parent().should('not.be.visible');
        cy.get(agreementUI.Section4NeighbourhoodDropdown).parent().should('be.visible');
    });


    it('PSPC Office Details Section: cities can be added and removed.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();

        agreementUI.AddNeighbourhood(testData.WorkplaceDetails.City);
        cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('a.label').contains(testData.WorkplaceDetails.City).should('exist');

        agreementUI.DeleteNeighbourhood(testData.WorkplaceDetails.City);
        cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('a.label').should('not.exist');
    });


    it('PSPC Office Details Section: Ensure user can\'t proceed without at least one city listed.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();

        /* Need this assertion to force it to wait for the transition to finish before continuing again so that the form validation will work correctly.
        Technically, you can create an IWA with no locations if you can hit Submit before the JS finishes loading the neighborhood form, but no
        human could possibly do it that quickly. */
        cy.get(agreementUI.SectionWorkTypeDetails).should('be.visible');

        agreementUI.Continue();
        cy.get(agreementUI.ErrorListItem).should('have.length', 1);
        cy.get(agreementUI.SectionWorkTypeDetails).should('be.visible');

        agreementUI.AddNeighbourhood(testData.WorkplaceDetails.City);
        agreementUI.Continue();

        cy.get(agreementUI.SectionSummary).should('be.visible');
    }); 


    it('Hybrid Details Section: Ensure non-pspc workplace, days per week, and city fields are showing.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();

        cy.get(agreementUI.Section4AddHomeAddressButton).should('be.visible');
        cy.get(agreementUI.Section4DaysInOfficeDropdown).parent().should('be.visible');
        cy.get(agreementUI.Section4NeighbourhoodDropdown).parent().should('be.visible');
    });


    it('Hybrid Details Section: Ensure user cant proceed without filling out all fields.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();

        cy.get(agreementUI.SectionWorkTypeDetails).should('be.visible');
        agreementUI.Continue();

        // This assertion should really be checking whether the error message is focused (not just visible), but I couldn't get that to work.
        cy.get(agreementUI.WorkplaceRequiredError).should('be.visible');

        agreementUI.AddAddress(testData.WorkplaceDetails.Address);
        cy.get(agreementUI.Section4AddressListItem).should('have.length', 1);
        agreementUI.Continue();

        cy.get(agreementUI.ErrorListItem).should('have.length', 1);
        cy.get(agreementUI.SectionWorkTypeDetails).should('be.visible');

        agreementUI.AddNeighbourhood(testData.WorkplaceDetails.City);
        agreementUI.Continue();

        cy.get(agreementUI.SectionSummary).should('be.visible');
    });


    it('Summary Section: Ensure agreement information is displayed.', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails);
        cy.get(agreementUI.SectionSummary).should('be.visible');

        cy.get(agreementUI.SummaryWorkType).should('contain', 'Hybrid');
        cy.get(agreementUI.SummaryDateRange).should('exist');
        cy.get(agreementUI.SummaryRemoteWorkplace).should('contain', testData.WorkplaceDetails.Address.streetAddress);
        cy.get(agreementUI.SummaryDaysInOffice).should('contain', testData.WorkplaceDetails.DaysInOffice);
        cy.get(agreementUI.SummaryNeighbourhood).should('contain', testData.WorkplaceDetails.City);
    });


    it('Ensure manager\'s dashboard displays submitted agreement and its status is "For review".', () => {
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails);
        cy.get(agreementUI.SectionSummary).should('be.visible');

        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'For review');
    });

    it('Ensure agreement can be reopened and status changes to "In progress" for employee and "Reopened" for manager.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails);
        cy.get(agreementUI.SectionSummary).should('be.visible');

        cy.log('Reopen the IWA and confirm the status is in progress.');
        agreementUI.Reopen();
        cy.get(agreementUI.StatusLabel).should('contain', 'In progress');

        cy.log('Log in as manager and confirm the status shows as reopened for the manager.');
        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');
        myEmployeesUI.VerifyEmployeeStatus(config.users.EmployeeUser.Username, 'Reopened');
    });

    it('Reopen remote work type: Ensure that the home address(es) appear correctly.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();
        agreementUI.CompleteSection4ForRemoteWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the home address(es) appear correctly.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();

        cy.get(agreementUI.Section4AddressListItem).should('contain', testData.WorkplaceDetails.Address.streetAddress);
        cy.get(agreementUI.Section4AddressListItem).should('contain', testData.WorkplaceDetails.Address.city);
        cy.get(agreementUI.Section4AddressListItem).should('contain', testData.WorkplaceDetails.Address.province);
        cy.get(agreementUI.Section4AddressListItem).should('contain', testData.WorkplaceDetails.Address.postalCode);
    });

    it('Reopen remote work type: Ensure that IWA can be changed to PSPC office worktype.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();
        agreementUI.CompleteSection4ForRemoteWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the reopened IWA can be submitted to supervisor as a PSPC Work Type.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();
        agreementUI.CompleteSection4ForPSPCWorkType(testData.WorkplaceDetails); 

        cy.log('Log in as manager and confirm the IWA has been changed to PSPC office worktype.');
        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');
        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        cy.get(agreementUI.SummaryWorkType).should('contain', 'All work hours in a PSPC workplace');
    });

    it('Reopen remote work type: Ensure that IWA can be changed to hybrid worktype.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();
        agreementUI.CompleteSection4ForRemoteWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the reopened IWA can be submitted to supervisor as a Hybrid Work Type.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails); 

        cy.log('Log in as manager and confirm the IWA has been changed to hybrid worktype.');
        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');
        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        cy.get(agreementUI.SummaryWorkType).should('contain', 'Hybrid model');
    });

    it('Reopen PSPC office work type: Ensure that PSPC office location(s) appear correctly.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();
        agreementUI.CompleteSection4ForPSPCWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the PSPC office location(s) appear correctly.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();

        cy.get(agreementUI.Section4NeighbourhoodDropdown).should('contain', testData.WorkplaceDetails.City)
    });

    it('Reopen PSPC office work type: Ensure that IWA can be changed to Remote work type.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();
        agreementUI.CompleteSection4ForPSPCWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the reopened IWA can be submitted to supervisor as a Remote work type.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();
        agreementUI.CompleteSection4ForRemoteWorkType(testData.WorkplaceDetails); 

        cy.log('Log in as manager and confirm the IWA has been changed to PSPC office worktype.');
        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');
        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        cy.get(agreementUI.SummaryWorkType).should('contain', 'All work hours in a non-PSPC workplace: "Remote work"');
    });

    it('Reopen PSPC office work type: Ensure that IWA can be changed to Hybrid work type.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();
        agreementUI.CompleteSection4ForPSPCWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the reopened IWA can be submitted to supervisor as a Hybrid work type.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.DeleteNeighbourhood(testData.WorkplaceDetails.City)
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails);   

        cy.log('Log in as manager and confirm the IWA has been changed to PSPC office worktype.');
        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');
        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        cy.get(agreementUI.SummaryWorkType).should('contain', 'Hybrid model');
    });

    it('Reopen hybrid work type: Ensure that the home address(es) appear correctly.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the home address(es) appear correctly.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();

        cy.get(agreementUI.Section4AddressListItem).should('contain', testData.WorkplaceDetails.Address.streetAddress);
        cy.get(agreementUI.Section4AddressListItem).should('contain', testData.WorkplaceDetails.Address.city);
        cy.get(agreementUI.Section4AddressListItem).should('contain', testData.WorkplaceDetails.Address.province);
        cy.get(agreementUI.Section4AddressListItem).should('contain', testData.WorkplaceDetails.Address.postalCode);
    });

    it('Reopen hybrid work type: Ensure that PSPC office location(s) appear correctly.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the PSPC office location(s) appear correctly.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();

        cy.get(agreementUI.Section4NeighbourhoodDropdown).should('contain', testData.WorkplaceDetails.City)
    });

    it('Reopen hybrid work type: Ensure that IWA can be changed to PSPC office worktype.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails);   
        
        cy.log('Reopen the IWA and confirm that the reopened IWA can be submitted to supervisor as a PSPC Work Type.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();
        agreementUI.DeleteNeighbourhood(testData.WorkplaceDetails.City)
        agreementUI.CompleteSection4ForPSPCWorkType(testData.WorkplaceDetails);

        cy.log('Log in as manager and confirm the IWA has been changed to PSPC office worktype.');
        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');
        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        cy.get(agreementUI.SummaryWorkType).should('contain', 'All work hours in a PSPC workplace');
    });

    it('Reopen hybrid work type: Ensure that IWA can be changed to remote worktype.', () => {
        cy.log('Create an IWA.');
        agreementUI.Start();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseHybridWorkType();
        agreementUI.CompleteSection4ForHybridWorkType(testData.WorkplaceDetails);   
        
        cy.log('Forward and approve IWA')

        cy.log('Reopen the IWA and confirm that the reopened IWA can be submitted to supervisor as a remote Work Type.');
        agreementUI.Reopen();
        agreementUI.CompleteSection1UserInfo();
        agreementUI.CompleteSection2TermsAndConditions();
        agreementUI.CompleteSection3ChooseRemoteWorkType();
        agreementUI.CompleteSection4ForRemoteWorkType(testData.WorkplaceDetails); 

        cy.log('Log in as manager and confirm the IWA has been changed to hybrid worktype.');
        config.LoginAsManagerUser();
        cy.visit(config.URL + '/en/MyEmployees');
        myEmployeesUI.ViewEmployeeIWA(config.users.EmployeeUser.Username);
        cy.get(agreementUI.SummaryWorkType).should('contain', 'All work hours in a non-PSPC workplace: "Remote work"');
    }); 
});