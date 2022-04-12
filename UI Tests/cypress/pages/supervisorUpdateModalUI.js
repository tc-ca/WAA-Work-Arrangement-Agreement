import { Select } from '../helpers/Utils';

const supervisorUpdateModalUI = {
    Modal: Select('supervisor-update-modal'),
    CloseButton: Select('close-supervisor-modal'),
    PSPCDLink: Select('goto-adc-btn'),


    // actions the user can take
    Close: () => {
        cy.get(supervisorUpdateModalUI.CloseButton).click();
    },
};

export default {
    ...supervisorUpdateModalUI
};