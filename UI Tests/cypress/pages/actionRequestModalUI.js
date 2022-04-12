import { Select } from '../helpers/Utils';

const actionRequestModalUI = {
    Modal: Select('approve-modal'),
    ApproveButton: Select('approve-modal-btn'),
    DenyButton: Select('deny-modal-btn'),
    RecommendButton: Select('recommend-modal-btn'),
    CancelButton: Select('close-modal-btn'),
    Level4ManagerYesButton: Select('level4mgr-yes-radio'),
    Level4ManagerNoButton: Select('level4mgr-no-radio'),
    Comments: Select('modal-comments-textarea'),

    Approve: () => {
        cy.get(actionRequestModalUI.ApproveButton).click();
    },

    Deny: () => {
        cy.get(actionRequestModalUI.DenyButton).click();
    },

    Recommend: () => {
        cy.get(actionRequestModalUI.RecommendButton).click();
    },

    Cancel: () => {
        cy.get(actionRequestModalUI.CancelButton).click();
    },

    ChooseLevel4ManagerResponse: (l4Manager) => {
        if (l4Manager === 'yes') {
            cy.get(actionRequestModalUI.Level4ManagerYesButton).click();
        }
        else if (l4Manager === 'no') {
            cy.get(actionRequestModalUI.Level4ManagerNoButton).click();
        }
        else {
            cy.log('AnswerLevel4Manager expects "yes" or "no" as a parameter!');
        }
    },

    AddComments: (comments) => {
        cy.get(actionRequestModalUI.Comments).clear().type(comments);
    },

    ApproveRequest: () => {
        actionRequestModalUI.ChooseLevel4ManagerResponse('yes');
        actionRequestModalUI.AddComments('This is a test comment.');
        actionRequestModalUI.Approve();
    },

    DenyRequest: () => {
        actionRequestModalUI.ChooseLevel4ManagerResponse('yes');
        actionRequestModalUI.AddComments('This is a test comment.');
        actionRequestModalUI.Deny();
    },

    RecommendRequest: () => {
        actionRequestModalUI.ChooseLevel4ManagerResponse('no');
        actionRequestModalUI.AddComments('This is a test comment.');
        actionRequestModalUI.Recommend();
    }
};

export default {
    ...actionRequestModalUI
};