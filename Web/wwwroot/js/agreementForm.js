var ccc;
$(document).ready(function () {
    var lang = $('#lang').val();
    var hostUrl = window.location.href;
    var baseurl = hostUrl.substr(0, hostUrl.indexOf('/en/') + hostUrl.indexOf('/fr/') + 2);
    var protocal = hostUrl.substr(0, hostUrl.indexOf(':'));
    $('.ui.checkbox').checkbox();
    $(".approveAttest").checkbox({
        onChecked: function () {
            if ($('.attest').not(':checked').length == 0) {
                $('#sumbit-btn').removeAttr('disabled');
            }            
        },
        onUnchecked: function () {
            $('#sumbit-btn').attr('disabled', 'disabled');
        }
    });
    $('#virtual-work-div').popup(); //tooltips
    $('#tmx').popup();
    //inputmask("A9A 9A9");
    //$('#switch-language-btn').addClass('disabled');

    //
    $(".postcode").keyup(function () {
        var pc = $(this).val();
        if (pc.length == 4) {
            if (pc.charAt(3) != ' ') {
                pc = pc.substr(0, 3) + ' ' + pc.charAt(3);
            }
        }
        var l = pc.length;
        if (l % 2 == 0) {
            if (l < 4) {
                if (!pc.charAt(l - 1).match(/[0-9]/i)) pc = pc.substr(0, l - 1);
            }
            if (l > 4) {
                if (!pc.charAt(l - 1).match(/[a-z]/i)) pc = pc.substr(0, l - 1);
            }
        } else {
            if (l < 4) {
                if (!pc.charAt(l - 1).match(/[a-z]/i)) pc = pc.substr(0, l - 1);
            }
            if (l > 4) {
                if (!pc.charAt(l - 1).match(/[0-9]/i)) pc = pc.substr(0, l - 1);
            }
        }
        $(this).val(pc);
    });
    //$('#FileUploads').modal({
    //    allowMultiple: true,
    //    closable: false,
    //    onApprove: function () {
    //        var file = $('#upFile')[0].files[0];
    //        if (file == null) return false;
    //        if (file.size / 1024000 > 10) {

    //            $("#msg-err-size").show();
    //            return false;
    //        }
    //        if (file.type !== 'application/pdf') {
    //            $("#msg-err-file-type").show();
    //            return false;
    //        }
    //        var id = $('#EmpAgreement_AgreementId').val();
    //        var files = new FormData();
    //        files.append('id', id);
    //        files.append('upFile', file);        
    //        $('.ui.inverted.dimmer').addClass('active');
    //        $.ajax({
    //            url: baseurl + 'api/agreement/upload/',
    //            type: 'POST',
    //            enctype: 'multipart/form-data',
    //            data: files,
    //            processData: false,  
    //            contentType: false,
    //            timeout: 60000,
    //            success: function (response) {                   
    //                var id = response;
    //                if (id > 0) {
    //                    $('#approvalDoc').empty();
    //                    var link = ("<a download='").concat(file.name, "' href='/en/agreement/viewdoc?id=", id, "' ><span>", file.name, "</span></a>");                       
    //                    $('#approvalDoc').append(link);
    //                    return true;
    //                }
    //                else {
    //                    $("#msg-err-upload").show();
    //                    $('.ui.inverted.dimmer').removeClass('active');
    //                    return false;
    //                }
    //            },
    //            error: function (jqXHR, textStatus, errorThrown) {
    //                //alert(jqXHR.error);
    //                $("#msg-err-upload").show();
    //                $('.ui.inverted.dimmer').removeClass('active');
    //                return false;
    //            }
    //        });
    //        return false;
    //    }
    //}).modal('attach events', '#test-btn', 'show');
    //$('#test-btn').click(function () {
    //    $(".message").hide();
    //    $('#upFile').val(null);
    //    $("#fileName").val('');
    //});
    //$("#upFile").on('change', function () {
    //    $(".message").hide();
    //    $("#fileName").val(this.files[0].name);
    //});

    //$('#approvalDoc').one('DOMSubtreeModified', function () {
    //    $('.ui.inverted.dimmer').removeClass('active');
    //    $('.ui.negative.button').click();
    //    var btn_txt = lang == "en" ? "Change File" : "Changer de Fichier";
    //    $('#test-btn').text(btn_txt);
    //    if ($("#approveAttest").checkbox('is checked')) {
    //        $('#sumbit-btn').removeAttr('disabled');
    //    }
    //});


    $('#manager-modal').modal({
        allowMultiple: true,
        closable: false,
        onApprove: function () {
            var recId = $("#hfUserId").val();
            var action = $('#decisions').dropdown('get value');
            $('#save-manager-btn').addClass('disabled');
            if (action === "r") {
                $.ajax({
                    url: baseurl + 'api/agreement/updaterecommender',
                    data: { "recommenderid": recId },
                    type: "POST",
                    dataType: "json",
                    success: function (response) {
                        if (response == true) {
                            $('#recomm-name').text($("#txtUserName").val());
                            $('#EmpAgreement_RecommenderId').val(recId);
                            $('#RecommenderFullName').val($("#txtUserName").val());

                            return true;
                        } else {
                            //error
                            alert("Error to update the recommender. Please try again.");
                            return false;
                        }
                        // $("#txtUserName").val('');
                        // $("#hfUserId").val('');
                    },
                    error: function (response) {
                        //alert(response.responseText);
                        return false;
                    },
                    failure: function (response) {
                        //alert(response.responseText);
                        return false;
                    }
                })
            } else {                
                $('#return-to-name').text($("#txtUserName").val());
                $('#EmpAgreement_RecommenderId').val(recId);
                $('#RecommenderFullName').val($("#txtUserName").val());
            }
            //return false;
        }
    }).modal('attach events', '.select-recommender-btn', 'show');
    var calText = {
        days: ['S', 'M', 'T', 'W', 'T', 'F', 'S'],
        months: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        monthsShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        today: 'Today',
        now: 'Now',
        am: 'AM',
        pm: 'PM'
    };
    if (lang == "fr") {
        calText = {
            days: ['D', 'L', 'M', 'M', 'J', 'V', 'S'],
            months: ['Janvier', 'Février', 'Mars', 'Avril', 'Mai', 'Juin', 'Juillet', 'Août', 'Septembre', 'Octobre', 'Novembre', 'Decembre'],
            monthsShort: ['Jan', 'Fev', 'Mar', 'Avr', 'Mai', 'Juin', 'Juil', 'Aou', 'Sep', 'Oct', 'Nov', 'Dec'],
            today: 'Aujourd\'hui',
            now: 'Maintenant',
            am: 'AM',
            pm: 'PM'
        }
    }

    $('#rangestart').calendar({
        type: 'date',
        initialDate: null,
        onChange: function (date, text) {
            var edate = datePlusOneYear(date);
            $('#rangeend').calendar('clear');
            $('#rangeend').calendar('set maxDate', edate);
            $('#rangeend').calendar('set minDate', date);
        },
        formatter: {
            date: function (date, settings) {
                return dateFormat(date);
            }
        },
        text: calText
    });
    var iniStartDate = $('#rangestart').calendar('get date');
    if (!iniStartDate.getMonth) {
        iniStartDate = new Date();
    }
    $('#rangeend').calendar({
        type: 'date',
        minDate: iniStartDate,
        maxDate: datePlusOneYear(iniStartDate),
        onSelect: function (cEnd, mode) {
            if (mode == 'day') {
                $('#msgDateOverlap').hide();
                $('#msgDateGap').hide();
                var sdate = $('#active-start-date').val();
                var edate = $('#active-end-date').val();
                if (sdate != null && edate != null) {
                    var cStart = $('#rangestart').calendar('get date').toLocaleDateString();                    
                    var aEnd = new Date(edate);
                    aEnd.setDate(aEnd.getDate() + 1);
                    var edate2 = aEnd.toLocaleDateString();
                    var newStart = Date.parse(new Date(cStart));
                    var oldEnd = Date.parse(edate2);
                    if (newStart < oldEnd) {
                        $('#msgDateOverlap').show();
                    } else if (newStart > oldEnd) {
                        $('#msgDateGap').show();
                    }
                }
            }
        },
        formatter: {
            date: function (date, settings) {
                return dateFormat(date);
            }
        },
        text: calText
    });


    //load a record intialize MyAgreement.HybridOptionId
    var workType_val = $("input[name='MyAgreement.WorkTypeId']:checked").val();
    $("#telework-addr-section").hide();
    $("#telework-addr-ctrl").hide();
    if (workType_val == "1") {
        $("#schedules-div").hide();
        $("#telework-addr-ctrl").show();
        if ($("#adhoc-cbx").checkbox('is checked')) {
            $("#telework-addr-section").show();
        }
        $(".hybrid_option").checkbox('uncheck');
    } else if (workType_val == "2") {
        $("#schedules-div").show();
        $("#fixed-schedules-div").show();
        $("#telework-addr-section").show();

    } 
    else {
        $("#schedules-div").hide();
    }
    $("#adhoc-addr").change(function () {
        clearTeleworkAddr();
        if (this.checked) {
            $("#telework-addr-section").show();
        } else {
            $("#telework-addr-section").hide();
        }
    });
    function clearTeleworkAddr() {
        $("#twStreet").val("");
        $("#twCity").val("");
        $("#twProvince").val("");
        $("#twPost").val("");
    }


    $(".WorkType").change(function () {
        $("#adhoc-addr").prop("checked", false);
        $("#telework-addr-section").hide();
        $("#telework-addr-ctrl").hide();
        $("#fixed-schedules-div").hide();
        $("#variable-schedule-txt").val("");
        $('#MyAgreement_TeleworkSchedule').val("0000000");

        var radioValue = $("input[name='MyAgreement.WorkTypeId']:checked").val();
        if (radioValue == '1') {
            $("#schedules-div").hide();
            $("#telework-addr-ctrl").show();
            clearTeleworkAddr();
            $(".hybrid_option").checkbox('uncheck');

        } else if (radioValue == '2') {

            $("#schedules-div").show();
            $("#fixed-schedules-div").show();
            $("#telework-addr-section").show();
           
        } 
    });

    //TC work sites
    $("#SelectedRegion").change(function () {
        var regionid = $(this).val();
        var select_option = (lang === 'en' ? 'Select worksite' : 'Sélectionnez le lieu de travail');
        $("#sites").addClass("loading");
        $.ajax({
            url: baseurl + 'api/agreement/' + lang + '/worksites/' + regionid,
            type: 'get',
            dataType: 'json',
            success: function (response) {
                var len = response.length;
                $("#worksites").empty();
                $("#worksites").append("<option value='-1'>" + select_option + "</option>");
                for (var i = 0; i < len; i++) {
                    var id = response[i]['id'];
                    var name = response[i]['name'];

                    $("#worksites").append("<option value='" + id + "'>" + name + "</option>");

                }
            }
        });
    });
    var virtual = $("#virtual-ind").val();
    if (virtual > 0) {
        $("#virtual-work-div").show();
    } else {
        $("#virtual-work-div").hide();
    }

    $("#worksites").change(function () {
        $("#virtual-work-div").hide();
        $("#virtual-ind").val(0);
        var siteid = $(this).val();
        if (siteid !== '-1') {
            var geocode = $("#user-geo-code").val();
            var regionid = $("#SelectedRegion").val();
            var provincelist = JSON.parse($("#geo-code").val());

            if (geocode === '3506008' || geocode === '2481017') {
                if (regionid !== 'A') {
                    $("#virtual-work-div").show();
                    $("#virtual-ind").val(1);
                }
            } else if (geocode.substr(0, 2) != provincelist[siteid]) {
                $("#virtual-work-div").show();
                $("#virtual-ind").val(1);
            }
        }

    });


    // Update manager
    $('#supervisor-update-btn').click(function () {
        $('#save-manager-btn').addClass('disabled');
        $('#approver-modal')
            .modal('setting', 'transition', 'scale')
            .modal('show');
    });

    $('#manager-close-btn').on('click', function () {
        $("#manager-form").form('reset');
        $("#txtUserName").val("");
        $('#approver-modal').modal('hide');
        setTimeout(function () { $("#manager-form > .error-summary-display").hide() }, 1000);
    });

    $("#txtUserName").keydown(function () {
        $('#save-manager-btn').addClass('disabled');
    });
    $("#txtUserName").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: baseurl + 'api/agreement/searchmanager',
                data: { "prefix": request.term },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $("#hfUserId").val(i.item.value);
            $("#txtUserName").val(i.item.label);
            $('#save-manager-btn').removeClass('disabled')
            $('#save-manager-btn').prop('disabled', false)
            return false;
        },
        minLength: 3,
        appendTo: "#manager-form"
    });
    $(".addr").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: baseurl + 'api/address/lookup',
                data: { "addr": request.term },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            populateAddr(i.item.value, e.target.id.substr(0, 2));
            return false;
        },
        minLength: 4,
        appendTo: "#address_section"
    });
    function populateAddr(addrId, el) {
        $.ajax({
            url: baseurl + 'api/address/retrieve',
            data: { "addrId": addrId },
            type: "POST",
            dataType: "json",
            success: function (data) {
                $('#' + el + 'Street').val(data["Line1"]);
                $('#' + el + 'City').val(data["City"]);
                $('#' + el + 'Province').val(data["ProvinceCode"]);
                $('#' + el + 'Post').val(data["PostalCode"]);
            },
            error: function (response) {
                $('#' + el + 'Street').val("");
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    }
    // INITIALIZE FORM
    var currentStep = 1;

    if ($('#ShowStartAgreement').val() ==='False') {
        switchNext();
        currentStep += 1;
    }

    $('#previous').hide();
    $('#SelectedNeighborhoods').dropdown({ selectOnKeydown: false });


    // CONTINUE BUTTON
    $('#next').on('click', function (e) {

        // Disable the language button once the user move beyond the first form screen.
        $('#switch-language-btn').addClass('disabled');
        if ($('#my-agreement-form').form('validate form')) {
            // If on step 1, run switchNext, increment the step and stop there.
            if (currentStep === 1) {
                switchNext();
                currentStep += 1;
            } else if (currentStep == 2) {
                $("#approverConfirm").modal({
                    allowMultiple: true,
                    closable: false,
                    onApprove: function () {
                        switchNext();
                        currentStep += 1;
                    }
                }).modal('show');
            } else if (currentStep === 3) {
                var inComplete = false;
                var inComplete2 = false;
                var countValue = 0;
                var countNoValue = 1;
                $('.altAddr0').each(function () {
                    countValue += this.value.length;
                    countNoValue *= this.value.length;
                })
                inComplete = (countValue > 0 && countNoValue == 0);
                countValue = 0;
                countNoValue = 1;
                $('.altAddr1').each(function () {
                    countValue += this.value.length;
                    countNoValue *= this.value.length;
                })
                inComplete2 = (countValue > 0 && countNoValue == 0);
                if (inComplete || inComplete2) {
                    $('.ui.modal.altAddrWarning').modal('setting', 'closable', false).modal('show');
                    if (inComplete) $('.altAddr0')[0].focus();
                    else $('.altAddr1')[0].focus();
                }
                else {
                    switchNext();
                    currentStep += 1;
                }
            }
            // If 1 < currentStep < 6, run switch next and increment the step only if the form validates.
            else if (currentStep < 6) {

                console.log('cs', currentStep);
                if (currentStep === 5) {
                    var ansNo = false;
                    $(".OhsChecklist:checked").each(function () {
                        if ($(this).val() == "no") {
                            ansNo = true;
                            return false;
                        };
                    });
                    if (ansNo) $('.ui.modal.ohsConfirm').modal('setting', 'closable', false).modal('show');
                }
                if (currentStep === 4) {
                    var teleworkType = $("input[name='MyAgreement.WorkTypeId']:checked").val();
                    if (teleworkType === '1' && !$("#adhoc-cbx").checkbox('is checked')) {
                        switchNext();
                        currentStep += 1;
                        console.log('cs#', currentStep);
                    }
                }
                switchNext();
                currentStep += 1;
            }

            // If on step 4, validate addresses/neighbourhoods as required and submit if possible.
            else {
                $('#my-agreement-form').addClass('loading');
                $('#next').addClass('disabled').find('.icon').removeClass().addClass('loading spinner icon');

                $('#my-agreement-form').submit();
            }
        }

    });


    // PREVIOUS BUTTON

    $('#previous').on('click', function (e) {
        var step = currentStep;
        var adjust = 0;
        var lang = $('#lang').val();
        console.log('showing', '#content-' + (step - 1), 'hiding', '#content-' + step);
        var teleworkType = $("input[name='MyAgreement.WorkTypeId']:checked").val();
        if (currentStep === 6 && teleworkType === '1' && !$("#adhoc-cbx").checkbox('is checked')) {
            currentStep -= 1;
            adjust = 1;
        }

        // Animate the transition.
        $('#content-' + step).transition({
            animation: 'fade',
            duration: '1ms',
            onComplete: function () {
                $('#content-' + (step - 1 - adjust)).transition('fade', '1ms');
                document.querySelector('#my-agreement-form').scrollIntoView();
                $('#my-agreement-form').focus();
                if (step == 6) {
                    document.getElementById('next-btn-txt').textContent = lang == 'en' ? 'Continue' : 'Continuer';
                }
            }
        });


        // If returning to step 1, remove Previous button and re-enable the language button.
        if (step - 1 === 1) {
            $('#previous').hide();
            $('#next').removeClass('disabled');
            $('#switch-language-btn').removeClass('disabled');
        }

        // Clean up any current errors.
        $('.error-summary-display').hide();

        jQuery.each($('.field.error').filter(':visible'), function (i, field) {
            $(field).removeClass('error');
        });

        currentStep -= 1;
    });


    // START AGREEMENT BUTTON

    //$('#start').on('click', function () {
    //    $.ajax({
    //        url: baseurl + 'api/agreement/create',
    //        type: 'POST',
    //        success: function () {
    //            window.location.reload();
    //        }
    //    });
    //});


    // WORK TYPE RADIO BUTTONS

    //$('input[name="MyAgreement.WorkTypeID"]').on('change', function () {
    //    // setupFormBasedOnWorkType(); //No more need this page
    //});


    // DISALLOW ENTER KEYPRESS ON INPUTS TO SUBMIT FORM
    $('form input').not('.search').keydown(function (e) {
        if (e.keyCode == 13) {
            $('#next').click();
            return false;
        }
    });


    // ANIMATE THE STATUS
    window.setTimeout(function () {
        $('.status-display').transition({
            animation: 'fade up',
            duration: 500,
            onComplete: function () {
                $('.status-display .label:last-child').transition('pulse');
            }
        });
    }, 350);


    // MISC FUNCTIONS
    function datePlusOneYear(date) {

        //alert(date);
        //if (date.val() === undefined) return new Date();
        var day = date.getDate();
        var month = date.getMonth();
        var year = date.getFullYear() + 1;
        var newDate = new Date(year, month, day);
        return newDate;
    }
    function dateFormat(date) {
        if (!date) return '';
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        return year + '-' + month + '-' + day;
    }
    function switchNext() {
        var step = currentStep;
        var lang = $('#lang').val();
        console.log('showing', '#content-' + (step + 1), 'hiding', '#content-' + step);

        // Animate the transition.
        $('#content-' + step).transition({
            animation: 'fade',
            duration: '1ms',
            onComplete: function () {
                $('#content-' + (step + 1)).transition('fade', '1ms');
                document.querySelector('#my-agreement-form').scrollIntoView();
                $('#my-agreement-form').focus();
                $('#previous').show();
                if (step == 1) {
                    if ($('#disableNextBtn').val() === 'True') {
                        $('#next').addClass('disabled');

                    } else {
                        $('#next').removeClass('disabled');
                    }
                } 
                /* If moving into the Work Type Details screen, change the Continue button text to Submit and 
                 * initialize address list display. */
                else if (step == 5) {
                    document.getElementById('next-btn-txt').textContent = lang == 'en' ? 'Submit' : 'Soumettre';
                }
            }
        });
    }


});