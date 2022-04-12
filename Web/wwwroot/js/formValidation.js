// use SUI form validation
setupFormValidation();
function setupFormValidation() {
    // custom rule for required fields that can be hidden (and then the validation doesnt apply)
    $.fn.form.settings.rules.requiredIfVisible = function (value) {
        var $elem = $(this);

        if ($elem.is(':visible') || $elem.parents('.ui.dropdown').is(':visible')) { // sui dropdown hides the select element, so we check for its parent to be visible in this case
            // checkboxes 
            if ($elem.is(':checkbox')) { // checkbox is checked, valid; not checked, invalid
                return $elem.is(':checked');
            }
            else {
                if ($elem.is(':radio')) { // if one of the radio buttons has a value, valid; otherwise, invalid  
                    return $('input[name="' + $elem.attr('name') + '"]:checked').val() !== undefined;
                }
                else {
                    // input has value, valid; empty, invalid
                    return !($elem.val().length === 0 || $elem.val() == -1);
                }
            }
        }
        else { // field is hidden, valid
            return true;
        }
    };
    $.fn.form.settings.rules.checkboxGroup = function (value) {
        //var allChecked = $('input[type=checkbox]').not(':checked').length === 0;
        var $elem = $(this);
        if ($elem.is(':visible')) { 
            return $(".optional:checked").length>0;
        }
        else { // field is hidden, valid
            return true;
        }
    };

    /* run for each form */
    $('.ui.form').each(function (index, elem) {

        var $form = $(this);

        if ($form.find('.error-summary-display, .error-message-list, .error-text').length !== 3) {
            //console.warn("Form validation error:", $form.attr('id'));
            //console.warn("An error summary must be displayed to use automatic form validation.  It needs to include .error-summary-display, .error-message-list, .error-text");
            return;
        }
        var config = {};
        // add validations for required fields
        $form.find('[data-val-required]').each(function (index, elem) {
            var id = $(elem).attr('id');
            var name = $(elem).attr('name');
            if (!$(elem).hasClass("optional")) {

                config[id] = {
                    identifier: name,
                    rules: [
                        {
                            type: 'requiredIfVisible',
                            prompt: $(elem).data('val-required')
                        }
                    ]
                };
                if (!$(elem).hasClass("termChecks")) {
                    $(this).attr('aria-required', true);
                    $('#' + id).parents('.field').addClass('required');
                }
                
            } else {
                config[id] = {
                    identifier: name,
                    rules: [
                        {
                            type: 'checkboxGroup',
                            prompt: 'Please select one option at lease'
                        }
                    ]
                };
                $(this).attr('aria-required', true);
               //$('#' + id).parents('.field').addClass('required');
            }

        });
        // add validations for requiredIfVisible fields 
        $form.find('[data-val-requiredIfVisible]').each(function (index, elem) {
            //
           if (!$(elem).hasClass("optional")) {
                var id = $(elem).attr('id');
                var name = $(elem).attr('name');
                config[id] = {
                    identifier: name,
                    rules: [
                        {
                            type: 'requiredIfVisible',
                            prompt: $(elem).data('val-requiredIfVisible')
                        }
                    ]
                };
                $(this).attr('aria-required', true);
                    $('#' + id).parents('.field').addClass('required');
            }
        });
        $form.find('.OhsChecklist').each(function (index, elem) {//.OhsChecklist
            var id = $(elem).attr('id');
            var name = $(elem).attr('name');
            config[id] = {
                identifier: name,
                rules: [
                    {
                        type: 'requiredIfVisible',
                        prompt: 'Please confirm terms'
                    }
                ]
            };
        });

        $form.find('#variable-schedule-txt').each(function (index, elem) {
            var id = $(elem).attr('id');
            var name = $(elem).attr('name');
            config[id] = {
                identifier: name,
                rules: [
                    {
                        type: 'requiredIfVisible',
                        prompt: 'Please enter your schedeule details'
                    }
                ]
            };
        });
        console.log(config);
        // give sui our model's validation rules 
        // this reads the `data-val` and `data-val-required` attributes that the asp-for helper adds to the markup
        // see https://fomantic-ui.com/behaviors/form.html for more information
        $form.form({
            fields: config,
            keyboardShortcuts: false, // fix bug with enter key press
            inline: false,
            onFailure: function () {
                var errorCount = 0;
                var items = "";
                var lastInput = "";
                var inputID = "";
                var ohsList = false;
                var groupCheckbox = false;
                var scheduleCheckbox = false;
                jQuery.each($form.find('.field.error').filter(':visible'), function (i, field) {
                    //if (errorCount > 0) { fieldList += ", "; }
                    var $field = $(field);                    
                    if ($field.find('input, radio').hasClass("OhsChecklist") && !ohsList) {                       
                        ohsList = true;
                        errorCount++;
                    } else if ($field.find('input, checkbox').hasClass("optional")) {                        
                        groupCheckbox = true;
                        errorCount++;
                    } else if ($field.find('input, select').attr('name') !== lastInput) { // only show one message for radios
                        lastInput = $field.find('input, select').attr('name');
                        inputID = $field.find('input, select').attr('id');
                        if (!(ohsList || groupCheckbox || scheduleCheckbox)) {
                            if ($field.find('input').is(':radio')) {
                                items += '<li data-testid="error-list-item"><button type="button" class="ui inverted red tertiary button error-link" data-target="' + inputID + '" > ' + $field.parents('fieldset').find('legend').text().trim() + "</button></li>";
                            }
                            else {
                                items += '<li data-testid="error-list-item"><button type="button" class="ui inverted red tertiary button error-link" data-target="' + inputID + '">' + $field.find('label').text().trim() + "</button></li>";
                            }
                        }
                        errorCount++;
                    }
                });
                var lang = $('#lang').val();
                var message = "";
                $('.field-error-list').remove();
                var fieldList = document.createElement("ul");
                fieldList.classList.add('field-error-list');
                //if not term list
                //message = "please check all conditions";
                fieldList.innerHTML = items;
               
                if (errorCount == 1) {
                    message = $form.find('.single-error-summary').val();
                }
                else {
                    message = $form.find('.multiple-error-summary').val();
                }
                message = message.replace('{numberOfErrors}', errorCount);

                if (ohsList) message = (lang == 'en' ? 'Please check all conditions' : 'Veuillez cocher toutes les conditions');
                if (groupCheckbox || scheduleCheckbox) message = (lang == 'en' ? 'Please select at least one option' : 'Veuillez sélectionner au moins une option');
                $form.find('.error-text').text(message);
                $form.find('.error-message-list').append(fieldList);

                if (errorCount == 0) {
                    $form.find('.error-summary-display').hide();
                }
                else {
                    $form.find('.error-summary-display').show();
                    $([document.documentElement, document.body]).animate({
                        scrollTop: $(".error-summary-display").offset().top
                    }, 500);
                    $('.error-text').attr('tabindex', '0');
                    $('.error-text').focus();
                }

                return false;
            },
        });

        // remove error display as user fixes errors
        $form.on('change paste', ':input', function (e) {

            if ($form.form('is valid')) {
                $form.find('.error-summary-display').hide();
            }
        });
    });


    /* allow clicking of individial error messages */
    $('body').on('click', '.error-link', function () {

        var $field = $('#' + $(this).data('target')).parents('.field');
        if ($field.first().find('.ui.dropdown:not(.search)').length > 0) {
            $field.first().find(':input, select').parents('.ui.dropdown').focus();
        } else {
            $field.first().find(':input').filter(':visible').first().focus();
        }
    });

    /* augment checkbox focus styles */
    $('.ui.checkbox input').on('focus', function () {
        $(this).parent('.ui.checkbox').addClass('focus');
    });
    $('.ui.checkbox input').on('blur', function () {
        $(this).parent('.ui.checkbox').removeClass('focus');
    });

}
