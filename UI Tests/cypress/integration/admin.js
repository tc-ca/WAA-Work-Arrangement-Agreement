import config from '../config/test.config.js';
import testData from '../config/test.data.js';
import api from '../config/test.api.js';
import adminUI from '../pages/adminUI.js';
import neighbourhoodAdminUI from '../pages/neighbourhoodAdminUI.js';
import agreementUI from '../pages/agreementUI.js';
import semantic from '../helpers/semantic.js';

context('Administration', () => {

    before(() => {        
        config.LoginAsAdminUser();
    });

    beforeEach(() => {
        api.CleanTestNeighbourhoods();
        cy.visit(config.URL + '/en/admin');
    });

    after(() => {
        api.CleanTestNeighbourhoods();
    });


    it.skip('Ensure that IWA report can be downloaded.', () => {
        // At some point, we could probably try to have cypress call the handler directly to download the file and
        // then make assertions against the response header data.
    });


    it('Ensure that PSPC Work Locations can be viewed.', () => {
        adminUI.GoToWorkLocations();
        cy.get(neighbourhoodAdminUI.AllRows).should('have.length.at.least', 1);
    });


    it('Ensure that PSPC Work Locations can be added.', () => {
        adminUI.GoToWorkLocations();
        
        neighbourhoodAdminUI.AddNewNeighbourhood(testData.Neighbourhood);
        neighbourhoodAdminUI.ReviewChanges();
        neighbourhoodAdminUI.SaveChanges();
        neighbourhoodAdminUI.WaitForLoadingToFinish();

        neighbourhoodAdminUI.VerifyNeighbourhood(testData.Neighbourhood);
    });


    it('Ensure that PSPC Work Locations can be modified.', () => {
        adminUI.GoToWorkLocations();
        
        cy.log('Add new neighbourhood...');
        neighbourhoodAdminUI.AddNewNeighbourhood(testData.Neighbourhood);
        neighbourhoodAdminUI.ReviewChanges();
        neighbourhoodAdminUI.SaveChanges();
        neighbourhoodAdminUI.WaitForLoadingToFinish();
        
        cy.log('Verify new neighbourhood...');
        neighbourhoodAdminUI.VerifyNeighbourhood(testData.Neighbourhood);

        cy.log('Clear filter...');
        neighbourhoodAdminUI.ClearTextFilter('NameEN');

        cy.log('Modify neighbourhood...');
        neighbourhoodAdminUI.ModifyNeighbourhood(testData.Neighbourhood, testData.NeighbourhoodModified);
        neighbourhoodAdminUI.ReviewChanges();
        neighbourhoodAdminUI.SaveChanges();
        neighbourhoodAdminUI.WaitForLoadingToFinish();

        cy.log('Verify modified neighbourhood...');
        neighbourhoodAdminUI.VerifyNeighbourhood(testData.NeighbourhoodModified);
    });


    context('Administration - Archived neighbourhoods', () => {

        before(() => {            
            api.CreateReportingStructure();
        });

        beforeEach(() => {
            config.LoginAsAdminUser();
        });

        afterEach(() => {
            api.CleanTestUserData(config.users.EmployeeUser.Username);
        })
        
        after(() => {
            api.CleanupReportingStructure();
        });
        

        it('Ensure that archived PSPC work locations are not removed from existing agreements.', () => {

            cy.log('Add test neighbourhood and confirm...');
            cy.visit(config.URL + '/en/admin/neighborhoods');
            neighbourhoodAdminUI.AddNewNeighbourhood(testData.Neighbourhood);
            neighbourhoodAdminUI.ReviewChanges();
            neighbourhoodAdminUI.SaveChanges();
            neighbourhoodAdminUI.WaitForLoadingToFinish();
            neighbourhoodAdminUI.VerifyNeighbourhood(testData.Neighbourhood);
    
            cy.log('Have employee create new agreement with test neighbourhood selected and confirm...')
            config.LoginAsEmployeeUser();
            cy.visit(config.URL + '/en/agreement');
    
            agreementUI.Start();
            agreementUI.CompleteSection1UserInfo();
            agreementUI.CompleteSection2TermsAndConditions();
            agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();
    
            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('input.search').click().type(testData.Neighbourhood.NameEN, { force: true });
            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('.menu').children('.item.selected').contains(testData.Neighbourhood.NameEN).should('exist');        
            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('.menu').children('.item.selected').contains(testData.Neighbourhood.NameEN).click();
    
            agreementUI.Continue();    
            cy.get(agreementUI.SummaryNeighbourhood).contains(testData.Neighbourhood.NameEN).should('exist');

            cy.log('Archive the test neighbourhood and confirm...');
            config.LoginAsAdminUser();
            cy.visit(config.URL + '/en/admin/neighborhoods');
            neighbourhoodAdminUI.FilterTextColumn('NameEN', testData.Neighbourhood.NameEN);
            neighbourhoodAdminUI.SetDropdownFieldByRowNumber(0, 'Archived', 'True');
            neighbourhoodAdminUI.ReviewChanges();
            neighbourhoodAdminUI.SaveChanges();
            neighbourhoodAdminUI.WaitForLoadingToFinish();

            neighbourhoodAdminUI.FilterTextColumn('NameEN', testData.Neighbourhood.NameEN);
            cy.get('.tabulator-cell[tabulator-field=Archived]').should('contain', 'True');

            cy.log('Confirm the test neighbourhood is still displayed for agreements where it is currently in use.');
            config.LoginAsEmployeeUser();
            cy.visit(config.URL + '/en/agreement');
            cy.get(agreementUI.SummaryNeighbourhood).contains(testData.Neighbourhood.NameEN).should('exist');

        });


        it('Ensure that archived PSPC work locations are not selectable from the dropdown in the agreement form.', () => {

            cy.log('Add test neighbourhood and confirm...');
            cy.visit(config.URL + '/en/admin/neighborhoods');
            neighbourhoodAdminUI.AddNewNeighbourhood(testData.Neighbourhood);
            neighbourhoodAdminUI.ReviewChanges();
            neighbourhoodAdminUI.SaveChanges();
            neighbourhoodAdminUI.WaitForLoadingToFinish();
            neighbourhoodAdminUI.VerifyNeighbourhood(testData.Neighbourhood);
    
            cy.log('Have employee create new agreement with test neighbourhood selected and confirm...')
            config.LoginAsEmployeeUser();
            cy.visit(config.URL + '/en/agreement');
    
            agreementUI.Start();
            agreementUI.CompleteSection1UserInfo();
            agreementUI.CompleteSection2TermsAndConditions();
            agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();
    
            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('input.search').click().type(testData.Neighbourhood.NameEN, { force: true });
            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('.menu').children('.item.selected').contains(testData.Neighbourhood.NameEN).should('exist');        
            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('.menu').children('.item.selected').contains(testData.Neighbourhood.NameEN).click();
    
            agreementUI.Continue();    
            cy.get(agreementUI.SummaryNeighbourhood).contains(testData.Neighbourhood.NameEN).should('exist');

            cy.log('Archive the test neighbourhood and confirm...');
            config.LoginAsAdminUser();
            cy.visit(config.URL + '/en/admin/neighborhoods');
            neighbourhoodAdminUI.FilterTextColumn('NameEN', testData.Neighbourhood.NameEN);
            neighbourhoodAdminUI.SetDropdownFieldByRowNumber(0, 'Archived', 'True');
            neighbourhoodAdminUI.ReviewChanges();
            neighbourhoodAdminUI.SaveChanges();
            neighbourhoodAdminUI.WaitForLoadingToFinish();

            neighbourhoodAdminUI.FilterTextColumn('NameEN', testData.Neighbourhood.NameEN);
            cy.get('.tabulator-cell[tabulator-field=Archived]').should('contain', 'True');

            cy.log('Confirm the test neighbourhood is still displayed for agreements where it is currently in use.');
            config.LoginAsEmployeeUser();
            cy.visit(config.URL + '/en/agreement');
            cy.get(agreementUI.SummaryNeighbourhood).contains(testData.Neighbourhood.NameEN).should('exist');

            cy.log('Reopen the agreement and confirm that the archived test neighbourhood is no longer selected and is not available in the dropdown.');
            agreementUI.Reopen();
            agreementUI.CompleteSection1UserInfo();
            agreementUI.CompleteSection2TermsAndConditions();
            agreementUI.CompleteSection3ChoosePSPCOfficeWorkType();
            cy.get(agreementUI.SectionWorkTypeDetails).should('be.visible');

            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('a.label.transition.visible').should('not.exist');

            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('input.search').click().type(testData.Neighbourhood.NameEN, { force: true });
            cy.get(agreementUI.Section4NeighbourhoodDropdown).siblings('.menu').children('.item.selected').should('not.exist');
            
        });

    });

});