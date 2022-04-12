/* 
 * Usage:
 * 
 */
(function ($) {

    $.fn.A11yDropdown = function (_config) {
        var $el = $(this);

        // defaults
        if ($el.attr('id') == undefined) {
            console.warn("Element doesnt exist",this, options, _config);
            return;
        }
        var options = $.extend({
            id: $el.attr('id'),
            required: false,
            searching: false,
            beforeSend: function (settings) {
                return settings;
            },
            url: '',
            onChange: function (value, text, $selectedItem) {
                return;
            },
        }, _config);


        // ensure library is being used correctly
        if (options.searching === true && options.url === '') {
            console.error("You must define a url to use a searching dropdown", options, _config);
            return;
        }

        if (options.searching) {
            $('#' + options.id).dropdown({
                useLabels: true,
                maxSelections: function () {
                    if ($('#' + options.id).attr('multiple')) {
                        return 1;
                    }
                    else {
                        return null;
                    }
                },
                saveRemoteData: false,
                clearable: true,
                selectOnKeydown: false,
                allowReselection: true,
                apiSettings: {
                    cache: false,
                    beforeSend: options.beforeSend,
                    url: options.url,
                    onResponse: function (response) {
                        $('#' + options.id).parent().find('.menu').empty();
                        searchResults = response.results;
                        return response;
                    }
                },
                message: {
                    noResults: $('#NoResultsFound').val()
                },
                showOnFocus: false,
                minCharacters: 3,
                delay: {
                    hide: 300,
                    show: 200,
                    search: 500,
                    touch: 50
                },
                onChange: options.onChange,
                onAdd: function (addedValue, addedText, $addedChoice) {
                    var record = searchResults.filter(function (obj) {
                        return obj.value === addedValue
                    })[0];
                    // a11y: add label text to SUI's generated inputs;
                    $('#' + options.id).siblings('input').attr('aria-label', $('label[for=' + options.id + ']').text() + ' - ' + addedText);


                    $(this).parent('.ui.dropdown').dropdown('hide');
                    if (options.onAdd != undefined) {
                        options.onAdd(addedValue, addedText, $addedChoice);
                    }
                },
                onRemove: function (removedValue, removedText, $removedChoice) {
                    $('#' + options.id).siblings('input').attr('aria-label', $('label[for=' + options.id + ']').text() + ' - ' + $('#' + options.id).dropdown('get default text'));
                },
                // show labels as popups in case text is to long to see
                onLabelCreate: function (value, text) {
                    var $label = $(this),
                        text = $label.text();

                    if (text.length > 40) {
                        $label
                            .attr('title', text)
                            .attr('data-variation', 'inverted')
                            .attr('data-position', 'bottom left')
                            .popup();
                    }

                    return $label;
                }
            });

            // add label text to SUI's generated inputs
            if ($('#' + options.id + ' option:selected').length > 0) {
                $('#' + options.id).siblings('input').attr('aria-label', $('label[for=' + options.id + ']').text() + ' - ' + $('#' + options.id + ' option:selected').text());
            } else {
                $('#' + options.id).siblings('input').attr('aria-label', $('label[for=' + options.id + ']').text() + ' - ' + $('#' + options.id).dropdown('get default text'));
            }

        } else {
            $('#' + options.id).dropdown();
        }


        // handle required
        if (options.required) {
            $('#' + options.id).siblings('input').attr('aria-required', true);
        }

        // handle focus indicator
        $('#' + options.id).siblings('input').on('focus', function () {
            $(this).parent('.ui.dropdown').addClass('focus');
        });
        $('#' + options.id).siblings('input').on('blur', function () {
            $(this).parent('.ui.dropdown').removeClass('focus');
        });

        // make clearing keyboard-friendly
        createClearableDropdown(options.id);

        return this;

    };

    function createClearableDropdown(id) {
        var _text = $('#lang').val() == 'en' ? 'clear field' : 'effacer champs';
        // remove sui's icon
        $('#' + id).siblings('.remove.icon').remove();
        // insert it back AFTER the input so it has the correct tab order
        $('#' + id).siblings('.text').after('<i tabindex="0" class="remove icon" aria-label="' + _text + '"></i>');
        // allow pressing enter on it to clear the input
        $('#' + id).siblings('.remove.icon').on('keypress', function (e) {
            if (e.which == 13) {
                e.preventDefault();
                $('#' + id).dropdown('clear');
                $('#' + id).siblings('input').focus();
            }
        });
    }


}(jQuery));
