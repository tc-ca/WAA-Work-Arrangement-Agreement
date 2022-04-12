
const config = {    
    Domain: 'PWGSC-TPSGC-EM',

    // PROD
    //URL: 'http://eti-iwa.spac-pspc.gc.ca',
    //baseURL: 'http://eti-iwa.spac-pspc.gc.ca',

    // TEST
    //URL: 'http://eti-iwa-test.spac-pspc.gc.ca',
    //baseURL: 'http://eti-iwa-test.spac-pspc.gc.ca',

    // DEV
    URL: 'https://eti-iwa-dev.spac-pspc.gc.ca',
    baseURL: 'https://eti-iwa-dev.spac-pspc.gc.ca',

    users: {
        AdminUser: {
            Username: 'cretesd2',
            Password: 'Canada01'
        },
        NonADCUser: {
            Username: 'TUDSB1',
            Password: 'dsbtest1234',
            DisplayName: 'DSGN SPA UsagerTest1 / DSB BPS TestUser1 (TPSGC/PWGSC)'
        },
        EmployeeUser: {
            Username: 'TUDSB3',
            Password: 'dsbtest1234',
            DisplayName: 'DSGN SPA UsagerTest3 / DSB BPS TestUser3 (TPSGC/PWGSC)'
        },
        ManagerUser: {
            Username: 'TUDSB4',
            Password: 'dsbtest1234',
            DisplayName: 'DSGN SPA UsagerTest4 / DSB BPS TestUser4 (TPSGC/PWGSC)'
        },
        DirectorUser: {
            Username: 'TUDSB5',
            Password: 'dsbtest1234',
            DisplayName: 'DSGN SPA UsagerTest5 / DSB BPS TestUser5 (TPSGC/PWGSC)'
        }
    },

    LoginAsAdminUser: () => {
        cy.clearCookies();
        cy.ntlmReset();
        cy.ntlm(config.baseURL, config.users.AdminUser.Username, config.users.AdminUser.Password, config.Domain);
    },
    LoginAsNonADCEmployeeUser: () => {
        cy.clearCookies();
        cy.ntlmReset();
        cy.ntlm(config.baseURL, config.users.NonADCUser.Username, config.users.NonADCUser.Password, config.Domain);
    },
    LoginAsEmployeeUser: () => {
        cy.clearCookies();
        cy.ntlmReset();
        cy.ntlm(config.baseURL, config.users.EmployeeUser.Username, config.users.EmployeeUser.Password, config.Domain);
    },
    LoginAsManagerUser: () => {
        cy.clearCookies();
        cy.ntlmReset();
        cy.ntlm(config.baseURL, config.users.ManagerUser.Username, config.users.ManagerUser.Password, config.Domain);
    },
    LoginAsDirectorUser: () => {
        cy.clearCookies();
        cy.ntlmReset();
        cy.ntlm(config.baseURL, config.users.DirectorUser.Username, config.users.DirectorUser.Password, config.Domain);
    },
    Login: (username, password) => {
        cy.clearCookies();
        cy.ntlmReset();
        cy.ntlm(config.baseURL, username, password, config.Domain);
    }
}

export default config;
