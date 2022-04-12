import { Select } from '../helpers/Utils.js';
import testData from '../config/test.data.js';

const neighbourhoodAdminUI = {
    BackButton: Select('neighbourhood-admin-back-btn'),
    TotalRecords: Select('total-records'),
    TotalRecordsDisplay: Select('total-records-display'),
    FilteredRecordsDisplay: Select('filtered-records-display'),
    AllRows: '.tabulator-row',
    AddRow: Select('addRowBtn'),
    ReviewChangesButton: Select('reviewBtn'),
    SaveChangesButton: Select('saveBtn'),
    RevertChangesButton: Select('revertBtn'),

    Back: () => {
        cy.get(neighbourhoodAdminUI.BackButton).click();
    },

    ReviewChanges: () => {
        cy.get(neighbourhoodAdminUI.ReviewChangesButton).click();
    },

    SaveChanges: () => {
        cy.get(neighbourhoodAdminUI.SaveChangesButton).click();
    },

    RevertChanges: () => {
        cy.get(neighbourhoodAdminUI.RevertChangesButton).click();
    },

    FindRow: (NameEN) => {
        return cy.get('.tabulator-cell[tabulator-field="NameEN"]').contains(NameEN).parent();
    },

    AddNewRow: () => {
        cy.get(neighbourhoodAdminUI.AddRow).click();
    },

    SetTextField: (rowID, fieldName, value) => {
        GetCell(rowID, fieldName).click().find('input').clear().type(value);        
    },

    SetTextFieldByRowNumber: (rowNumber, fieldName, value) => {
        cy.get('.tabulator-row').eq(rowNumber).find('[tabulator-field=ID]').then(($cell) => {
            const rowID = Cypress.$(cell).text();
            GetCell(rowID, fieldName).click().find('input').clear().type(value);
        });                
    },

    SetDropdownField: (rowID, fieldName, value) => {
        GetCell(rowID, fieldName).click();
        SelectDropdownItem(value);
    },

    SetDropdownFieldByRowNumber: (rowNumber, fieldName, value) => {
        cy.get('.tabulator-row').eq(rowNumber).find('[tabulator-field=ID]').then((cell) => {
            const rowID = Cypress.$(cell).text();
            GetCell(rowID, fieldName).click();
            SelectDropdownItem(value);
        });                
    },

    FilterTextColumn: (fieldName, searchText) => {
        GetHeader(fieldName).find('input').type(searchText);
        cy.wait(500);
    },

    FilterDropdownColumn: (fieldName, selectionText) => {
        GetHeader(fieldName).find('input').click();
        SelectDropdownItem(selectionText);
        cy.wait(500);
    },

    ClearTextFilter: (fieldName) => {
        GetHeader(fieldName).find('input').click().clear();
        cy.wait(500);
    },

    WaitForLoadingToFinish: () => {
        cy.get('.loading').should('not.exist');
    },

    AddNewNeighbourhood: (neighbourhoodData) => {
        cy.get(neighbourhoodAdminUI.AddRow).click();
        neighbourhoodAdminUI.SetTextField('NEW', 'NameEN', neighbourhoodData.NameEN);
        neighbourhoodAdminUI.SetTextField('NEW', 'NameFR', neighbourhoodData.NameFR);
        neighbourhoodAdminUI.SetDropdownField('NEW', 'Archived', neighbourhoodData.Archived);
    },

    ModifyNeighbourhood: (originalNeighbourhoodData, newNeighbourhoodData) => {
        neighbourhoodAdminUI.FilterTextColumn('NameEN', originalNeighbourhoodData.NameEN);
        neighbourhoodAdminUI.FindRow(originalNeighbourhoodData.NameEN).find('.tabulator-cell[tabulator-field="ID"]').invoke('text').then((id) => {
            neighbourhoodAdminUI.SetTextField(id, 'NameEN', newNeighbourhoodData.NameEN);
            neighbourhoodAdminUI.SetTextField(id, 'NameFR', newNeighbourhoodData.NameFR);
            neighbourhoodAdminUI.SetDropdownField(id, 'Archived', newNeighbourhoodData.Archived);
        });
    },

    VerifyNeighbourhood: (verifData) => {
        neighbourhoodAdminUI.FilterTextColumn('NameEN', verifData.NameEN);
        cy.get('.tabulator-row', {timeout: 15000}).should('have.length', 1);

        cy.get('.tabulator-cell[tabulator-field=NameEN]').should('contain', verifData.NameEN);
        cy.get('.tabulator-cell[tabulator-field=NameFR]').should('contain', verifData.NameFR);

        neighbourhoodAdminUI.ClearTextFilter('NameEN');
    },
}

function GetRow(id) {
    return cy.get('.tabulator-cell[tabulator-field="ID"]').contains(id).parent();
}

function GetCell(id, fieldName) {
    return GetRow(id).children('.tabulator-cell[tabulator-field="' + fieldName + '"]');
}

function GetHeader(fieldName) {
    return cy.get('.tabulator-col[tabulator-field=' + fieldName + ']');
}

function SelectDropdownItem(value) {
    cy.get('.tabulator-edit-select-list-item').contains(value).click();
}

export default {
    ...neighbourhoodAdminUI
}