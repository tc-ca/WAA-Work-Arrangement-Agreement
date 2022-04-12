import { Select } from '../helpers/Utils';
import semantic from '../helpers/semantic.js';

const addAddressModalUI = {
    Modal: Select('address-modal'),
    SaveButton: Select('save-address-btn'),
    CloseButton: Select('close-address-modal'),
    StreetAddress: Select('new-streetaddress-field'),
    City: Select('new-city-field'),
    Province: Select('new-prov-field'),
    PostalCode: Select('new-postalcode-field'),

    Close: () => {
        cy.get(addAddressModalUI.CloseButton).click();
    },

    Save: () => {
        cy.get(addAddressModalUI.SaveButton).click();
    },

    AddAddress: (address) => {
        cy.get(addAddressModalUI.StreetAddress).clear().type(address.streetAddress);
        cy.get(addAddressModalUI.City).clear().type(address.city);
        semantic.SelectFromDropdown(addAddressModalUI.Province, address.province);
        cy.get(addAddressModalUI.PostalCode).clear().type(address.postalCode);
        addAddressModalUI.Save();
    }
};

export default {
    ...addAddressModalUI
};