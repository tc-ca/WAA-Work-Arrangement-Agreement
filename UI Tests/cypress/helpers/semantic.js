const semantic = {
    SelectFromDropdown: (selector, value) => {
        cy.get(selector).parent().click();
        cy.get(selector).siblings('.menu').find('.item').contains(value).click();
    },

    ToggleCheckbox: (selector) => {
        cy.get(selector).parent().click();
    },
};

export default {
    ...semantic
};