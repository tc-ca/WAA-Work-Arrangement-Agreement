## Purpose
This project contains the integration tests for the the ERP app.


## Technology
The tests are written in JavaScript using the Cypress.io framework.


## Getting started
To try out the tests, you'll need:

* VSCode (lightweight editor, better than full VS for javascript projects): 
    * https://code.visualstudio.com/download
* node.js:  
    * https://nodejs.org/en/download/current/
* git bash:
    * https://github.com/git-for-windows/git/releases/download/v2.19.1.windows.1/Git-2.19.1-64-bit.exe


### Clone the repo
Open up Git bash and navigate to a folder where you want to put the code.
Type the following to get the latest project code:
`git clone https://bitbucket.tpsgc-pwgsc.gc.ca/scm/dsba/adcensus.git`


### Install dependencies (Cypress.io!)
`cd UITests`

`npm install`


### Run the Cypress UI
`npx cypress open`


## Getting started with Cypress
The following articles will help you understand how cypress works and how to use it:
* https://docs.cypress.io/guides/overview/why-cypress.html#In-a-nutshell
* https://docs.cypress.io/guides/getting-started/writing-your-first-test.html#Add-a-test-file
* https://docs.cypress.io/guides/core-concepts/introduction-to-cypress.html#Cypress-Is-Simple


## Best practices
* https://docs.cypress.io/guides/references/best-practices.html
* Using `data` attributes instead of CSS classes or IDs:
    * https://docs.cypress.io/guides/references/best-practices.html#Selecting-Elements
