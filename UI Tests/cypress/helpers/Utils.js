/**
 * Creates a selector using the data-testid of an element
 *
 * @param {string} id The data-testid to select
 */
export const Select = (id) => `[data-testid="${id}"]`;

export const Log = (message) => {
    cy.log('# ---------------------------------------------- ');
    cy.log(`> ${message}`);
    cy.log('# ---------------------------------------------- ');
};