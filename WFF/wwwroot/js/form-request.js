$(document).ready(function () {
    // hide empty tab panes
    $(".tab-pane").each(function (i, ctrl) {
        if ($(ctrl).find("input, table").length == 0) {
            $("a").filter(function () {
                return $.trim($(this).attr("href")) === "#" + $(ctrl).attr("id");
            }).hide();
        }
    });

    // SELECT USER MODAL
    // open modal (saves the control that open it, and clear the option selected in the select list)
    $('[id^=btnOpenSelectUser]').on('click', function (event) {
        $("#selectUserModal #selectUserNameCaller").val($(this).attr("id"));
        $("#selectUserNameList option:selected").prop("selected", false);
        $("#selectUserModal .validation-error").css('visibility', 'hidden');
    });

    // select user (pass the value to the corresponding textbox input and close the modal)
    $('#selectUserModal #btnSelectUser').on('click', function (event) {
        if ($("#selectUserModal #selectUserNameList option:selected").length == 0) {
            $("#selectUserModal .validation-error").css('visibility', 'visible');
            return;
        }

        var caller = $("#selectUserModal #selectUserNameCaller").val();
        var txtbx = $("#" + caller).parent().find("input[type='textbox']");
        var hdnEmail = $("#" + caller).parent().find("input[id^=email][type='hidden']");
        txtbx.val($("#selectUserNameList option:selected").text()).change();
        hdnEmail.val($("#selectUserNameList option:selected").attr("data-email"));

        $('#selectUserModal').modal('toggle');
    });

    // PARAMETER TABLE MODAL
    // change class to highlight selected row in modal
    $('.table-clickable').on('click', '.clickable-row', function (event) {
        $(this).addClass('active').siblings().removeClass('active');
    });

    // open modal (save the control that open it and clear option already selected)
    $('[id^=btnOpenModalParameter]').on('click', function (event) {
        var modalSelector = $(this).attr("id").replace("btnOpenModalParameter", "");
        $("#modalCaller" + modalSelector).val($(this).parents(".full-row,.half-row").first().find("input[type=textbox]").attr("id"));

        $("#modal" + modalSelector).find(".table-clickable tbody tr.active").removeClass('active');
        $("#modal" + modalSelector).find(".validation-error").css('visibility', 'hidden');
    });

    // select option (pass the value to the corresponding textbox input and close modal)
    $('[name=btnSelectOption]').on('click', function (event) {
        var modal = $(this).parents("div.modal");
        if (modal.find(".table-clickable tbody tr.active").length == 0) {
            modal.find(".validation-error").css('visibility', 'visible');
            return;
        }

        var caller = $(this).parent().find("input[type=hidden]").val();
        var colIndexToSave = modal.find(".table-clickable thead th[save=true]")[0].cellIndex;

        var txtbx = $("#" + caller).parent().find("input[type='textbox']");;
        var value = modal.find(".table-clickable tbody tr.active td")[colIndexToSave].textContent.trim();

        txtbx.val(value).change();

        var emailInput = $("#" + caller).parent().find("input[type=hidden][name^=email]");
        if (emailInput.length > 0) {
            var email = $(modal.find(".table-clickable tbody tr.active td")[colIndexToSave]).attr("data-email");
            emailInput.val(email);
        }

        modal.modal('toggle');
    });

    // convert date inputs element into datepickers
    $('.input-group.date').datepicker({
        format: "dd/M/yyyy",
        autoclose: true
    });

    // function for giving each click event to show/hide elements for radiobuttons and checkboxes
    var setHideDisplayEvents = function(control, attribute, funct) {
        var controlsToHide = control.attr(attribute);

        var radioButtonGroup = $("[name=" + control.attr("name") + "]");
        var controls = getHideShowControls(radioButtonGroup, attribute);

        control.on('click', function () {
            radiocheckClick(controlsToHide, controls, funct);
        });
    }


    // display/hide functionality for controls with "data-hidden" attribute set according hiddenFields node in the form xml configuration (using "display" which hides the element and remove the empty space)
    $("[data-hidden][type=checkbox],[data-hidden][type=radio]").each(function (control) {
        setHideDisplayEvents($(this), "data-hidden", removeColumns);
    });

    // change value of fields based on others field's options for controls with attributes that start with "data-change-" set according nodes in the form xml configuration (parameters section)
    $(':attrStartsWith("data-change")').each(function (control) {
        var control = $(this);
        var controlAttributes = control[0].attributes;

        // to-do: ES NECESARIO ESTE FOR??
        //for (var i = 0; i < controlAttributes.length; i++) {
        //    var attr = controlAttributes[i];
        //    if (attr.name.indexOf("data-change") > -1) {
        //        var attrSplit = attr.value.split("|");

        //        if (attrSplit.length > 1) {
                    var type = control.attr("type");

                    if (type == "radio" || type == "checkbox") {
                        control.on("click", function () {
                            var attributes = $(this)[0].attributes;
                            for (var j = 0; j < attributes.length; j++) {
                                if (attributes[j].name.indexOf("data-change") > -1) {
                                    var changeAttr = attributes[j].value.split("|");
                                    $("#" + changeAttr[0]).val(changeAttr[1]);
                                }
                            }
                        });
                    }
                    else if (type == "textbox") {
                        control.on("change", function () {
                            var attributes = $(this)[0].attributes;
                            for (var j = 0; j < attributes.length; j++) {
                                var changeAttr = attributes[j].value.split("|");

                                if (changeAttr[0] == $(this).val())
                                    $("#" + changeAttr[1]).val(changeAttr[2]);
                            }
                        });
                    }
        //        }
        //    }
        //}
    });

    // add filter by text functionality to select user modal window
    $(function () {
        $('#selectUserNameList').filterByText($('#filterUserNameList'));
    });

    // functionality required to attach files, required when there's two fields to be displayed based on a condition but only one shows at a time
    $(document).on("change", "input[type=file][id^=file]", function () {
        var ctrlId = $(this).attr("id");
        //var ctrlId = $("[name='fileAttachments:0']").attr("id");
        var inputFile = ctrlId.substring(4, ctrlId.length);

        var idSplit = $(this).parents("[class^=col]").find("input[type=file]").last().attr("id").split(":");
        //var idSplit = $("[name='fileAttachments:0']").parents("[class^=col]").find("input[type=file]").last().attr("id").split(":");
        var lastFileIndex = parseInt(idSplit[1]);

        var container = $("[id='container" + inputFile + "']").clone();
        var select = "[id$=':" + lastFileIndex + "']";

        container.find(select).addBack(select).each(function (i, ctrl) {
            $(ctrl).attr("id", $(ctrl).attr("id").replace(":" + lastFileIndex, ":" + (lastFileIndex + 1)));

            if ($(ctrl).attr("name") != undefined)
                $(ctrl).attr("name", $(ctrl).attr("name").replace(":" + lastFileIndex, ":" + (lastFileIndex + 1)));

            if ($(ctrl).attr("onclick") != undefined)
                $(ctrl).attr("onclick", $(ctrl).attr("onclick").replace(":" + lastFileIndex, ":" + (lastFileIndex + 1)));
        });

        container.find("input[type=textbox]").val(null);
        container.insertAfter("[id='container" + inputFile + "']");

        var file = $("[id='file" + inputFile + "']")[0].files[0].name;
        $("[id='" + inputFile + "']").val(file);
        $("[id='container" + inputFile + "']").show().trigger($.Event('resize'));
    });

    // moves up divs with replacing-field class 
    $(".row.replacing-field").each(function () {
        var row = $(this);
        row.css("margin-top", parseInt(row.prev("div.row").innerHeight()) * -1 + 1);

        $(this).prev("div.row").on("resize", function () {
            row.css("margin-top", parseInt($(this).innerHeight()) * -1 + 1);
        })
    });

    $("[data-visiblewhen]").each(function () {
        var ctrl = $(this);
        var ctrlId = $(this).attr("name");
        var visibleCondition = $(this).attr("data-visibleWhen");

        SetFieldsValidations(visibleCondition, ctrlId, "field");
        $(document).on('change', "input", function () {
            SetFieldsValidations(visibleCondition, ctrlId, "field");
        });
    });

    // set clockpicker definition
    $('.clockpicker').clockpicker({
        placement: 'top',
        align: 'left'
    });

    $(document).on("change", ".table.custom-field input", function() {
        updateCustomFullValue($(this));
    });

    $(document).on("click", ".custom-list-field .add-item a", function () {
        addItemToCustomList($(this));
    });

    return;
});

function SaveForm() {
    if ($("#closeRequestFormModal").is(":visible")) {
        $("#closeRequestFormModal").modal("toggle");
    }

    $(".lbl-error").removeClass("lbl-error");
    var isFormValid = $("#requestForm").valid();
    if (!isFormValid) {
        $("#requestForm input.error").filter("[type=checkbox]").parents("div").first().find("label").addClass("lbl-error");
        $("#requestForm input.error").filter("[type=radio]").parent("label").addClass("lbl-error");

        return;
    }

    OpenSavingLoadingModal();

    $.ajax({
        type: "POST",
        url: '../FormRequest/Save',
        data: getPostData(),
        processData: false,
        contentType: false,
        success: function (data) {
            window.location.href = data.redirectTo;
        },
        error: function (data, status) {
            console.log(data);
            console.log(status);
        }
    });
}

function getPostData(form) {
    var postData = new FormData();

    var formData = getFormData($("#requestForm"));
    var id = $("#Id").val();
    var reference = $("#Reference").val();
    var status = $("#Status").val();
    var statusId = $("#StatusId").val();
    var createdBy = $("#CreatedBy").val();
    var formId = $("#FormId").val();

    var comment = "";
    if ($("#modalComments").is(":visible")) {
        if ($("#modalComments textarea").val() == "") {
            $("#modalComments textarea").addClass("error");

            return;
        }

        comment = $("#modalComments textarea").val();
        $("#modalComments textarea").removeClass("error");
        $("#modalComments").modal("toggle");
    }

    postData.append("formData", JSON.stringify(formData));
    postData.append("viewModel.Reference", reference);
    postData.append("viewModel.Id", id);
    postData.append("viewModel.FormId", formId);
    postData.append("viewModel.StatusId", statusId);
    postData.append("viewModel.Status", status);
    postData.append("viewModel.CreatedBy", createdBy);

    postData.append("viewModel.Comment", comment);
    postData.append("viewModel.RequestFormId", id);

    if (form != undefined) {
        var newStatus = $(form + " #NewStatus").val();
        var newStatusDisplayName = $(form + " #NewStatusDisplayName").val();
        var sendEmailNotification = $(form + " #SendEmailNotification").val();
        var action = $(form + " #HistoryText").val();

        postData.append("viewModel.NewStatus", newStatus);
        postData.append("viewModel.NewStatusDisplayName", newStatusDisplayName);
        postData.append("viewModel.SendEmailNotification", sendEmailNotification);
        postData.append("viewModel.Action", action);
    }


    GetAttachements(postData);

    return postData;
}

function GetAttachements(formData) {
    var attachments = [];
    var attachment = {};
    var index = 0;

    var addAttachment = function (index, attachment) {
        formData.append("viewModel.Attachments[" + index + "].ControlId", attachment.ControlId.substring(4));
        for (var i = 0; i < attachment.Attachments.length; i++) {
            formData.append("viewModel.Attachments[" + index + "].Attachments[" + i + "]", attachment.Attachments[i]);
        }

        attachments.push(attachment);
    }

    $("#requestForm input[type=file]").each(function (i, ctrl) {
        if ($(ctrl).parent().find("input[type=textbox]:visible").length == 0 || $(ctrl).parent().find("input[type=hidden]").length > 0)
            return;

        var ctrlId = $(ctrl).attr("id");
        if (attachment.ControlId == undefined) {
            attachment.ControlId = ctrlId.split(":")[0];
            attachment.Attachments = [];
        }

        attachment.Attachments.push($(ctrl)[0].files[0]);

        var attachmentsNumber = $(ctrl).parents("[class^=col]").find("input[type=textbox]:visible").length - $(ctrl).parents("[class^=col]").find("input[type=hidden]").length;
        if (attachmentsNumber == attachment.Attachments.length) {
            addAttachment(index, attachment);
            attachment = {};
            index++;
        }
    });

    //attachments
}   

function ChangeRequestStatus(form, actionJSFunction, addCommentToList, cancelStatusChange) {
    if (cancelStatusChange != 'True' && !isRequestFormValid()) return;

    var comment = "";
    if ($("#modalComments").is(":visible")) {
        if ($("#modalComments textarea").val() == "") {
            $("#modalComments textarea").addClass("error");

            return;
        }

        comment = $("#modalComments textarea").val();
        $("#modalComments textarea").removeClass("error");
        $("#modalComments").modal("toggle");
    }

    if ($("#modalMessage").is(":visible")) {
        $("#modalMessage").modal("hide");
    }

    if (cancelStatusChange != 'True') {
        var emptyInputs = "";
        var actionRequiredFields = $(form).find("#RequiredFields").val();
        actionRequiredFields.split(",").forEach(function (control) {
            if (control == "") return;

            var isInputInvalid = false;
            var currentControl = $("[name=" + control + "]");

            switch (currentControl.attr("type")) {
                case "radio": case "checkbox":
                    isInputInvalid = !currentControl.is(":checked") && currentControl.css("visibility") != "hidden";

                    break;
                default:
                    isInputInvalid = currentControl.val() == "" && currentControl.css("visibility") != "hidden";
            }

            if (isInputInvalid) {
                if (emptyInputs == "")
                    emptyInputs = "Please complete the following fields:<br /><br />";

                var colContainer = currentControl.parents("div.col-sm-6");
                var parentSelector = colContainer.length > 0 ? "div.col-sm-6" : "div.col-sm-10";
                emptyInputs += "<div>- " + currentControl.parents(parentSelector).prev().find("label").html() + "</div>";
            }
        });

        if (emptyInputs.length > 0) {
            setTimeout(function () { OpenMessageModal("Warning Message", emptyInputs); }, 500);
            return;
        }
    }
    
    if (actionJSFunction != null && actionJSFunction != '') {
        var funct = actionJSFunction.split('(')[0];
        var params = actionJSFunction.split('(')[1].substring(0, actionJSFunction.split('(')[1].length - 1);
        window[funct](JSON.parse(params));

        if (cancelStatusChange == "True")
            return;
    }

    if (addCommentToList != null && addCommentToList != '' && comment != '') {
        AddItemToList(addCommentToList, comment, true);
    }

    OpenChangeStatusModal();

    $.ajax({
        type: "POST",
        url: '../FormRequest/ChangeStatus',
        data: getPostData(form),
        processData: false,
        contentType: false,
        success: function (data) {
            window.location.href = data.RedirectTo;
        },
        error: function (data, status) {
            console.log("fallaste!");
        }
    });
}

function isRequestFormValid() {
    var isFormValid = $($("#requestForm")).valid();
    if ($("#requestForm").length == 0) return true;

    if (!isFormValid) {
        $("#requestForm input[type=radio].error").parent("label").addClass("lbl-error");

        return false;
    }

    return true;
}

function openFileModal(inputFile) {
    inputFile = $("[id^='file" + inputFile + "'][type=file]").last();
    inputFile.trigger('click');
}

function removeModal(inputFile) {
    container = $("[id='file" + inputFile + "']").parent("div");
    OpenYesNoModal("WFF Notice", "Deleting attachement(s) cannot be undone. Do you want to proceed?", "RemoveContainer(container)");
}

function removeCustomListItemModal(ctrl) {
    container = $("tr[id='" + ctrl + "']");
    OpenYesNoModal("WFF Notice", "Deleting an item in the list cannot be undone. Do you want to proceed?", "RemoveContainer(container)");
}

function RemoveContainer(container) {
    container.remove();
    $("#modalMessage").modal('toggle');
}

function OpenChangeStatusModal() {
    $("#modalLoading").modal('toggle');
    $("#modalLoading #lblLoading").html('');
}

function OpenSavingLoadingModal() {
    $("#modalLoading").modal('toggle');
    $("#modalLoading #lblLoading").html('saving the request, please wait...');
}

function OpenCommentModal(saveFunction) {
    if ($("#modalMessage").is(":visible")) {
        $("#modalMessage").modal("toggle");
    }

    $("#modalComments").modal('toggle');
    $("#modalCommentsSave").attr("onclick", saveFunction);

    var maxLength = $("#modalComments textarea").attr("maxlength");
    $("#modalComments textarea").keyup(function () {
        var length = $(this).val().length;
        var length = maxLength - length;
        $('#modalComments .remaining-chars #chars').text(length);
    });
}

function getFormData() {
    // filter to remove files attachments fields (as it's being save in the database)
    var unindexed_array = $("#requestForm").find("input").filter(function (index, element) {
        var filesCondition = $(element).parent().find("input[type=file]").length == 0;

        var listCondition = true;
        if ($(element).attr("id") != undefined) {
            var isList = $(element).attr("id").indexOf(":");
            if (isList > -1 && $(element).is("[type=textbox]"))     {
                if ($("[name^='" + $(element).attr("id").split(":")[0] + "']").last().attr("id") == $(element).attr("id") && $(element).is(":empty") && $(element).val() == "")
                    listCondition = false;
            }
        }

        return filesCondition && listCondition;
    }).serializeArray();

    var indexed_array = {};
    $.map(unindexed_array, function (n, i) {
        if (unindexed_array.filter(function (item) { return item.name == n['name'] && item.value != ""; }).length > 1) {
            if (indexed_array[n['name']] == undefined)
                indexed_array[n['name']] = [];

            indexed_array[n['name']].push(n['value']);
        }
        else if (indexed_array[n['name']] == undefined || indexed_array[n['name']] == '') 
            indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

function ParseFieldsToFormData(text)
{
    var delimiters = [ "field", "role" ];
    text = text.replace("<![CDATA[", "").replace("]]>", "");

    for(var i = 0; i < delimiters.length; i++) {
        var delim = delimiters[i];
        var matches = text.match(new RegExp(delim + '\(([^)]+)\).', 'gi'));
        if (matches != null) {
            for (var j = 0; j < matches.length; j++) {
                var field = matches[j];
                field = field.replace(delim + "(", "");
                field = field.substring(0, field.length - 1);

                if (field.indexOf(',') > -1) {
                    var fieldSplit = field.split(',');
                    for (var k = 0; k < fieldSplit.length; k++) {
                        var item = fieldSplit[k];
                        var htmlField = $("#requestForm #" + item);

                        if (htmlField.length > 0) {
                            text = text.replace(matches[j], htmlField.val());
                            break;
                        }
                    }
                }
                else {
                    var htmlField = $("#requestForm #" + field);
                    if (htmlField.length > 0)
                        text = text.replace(matches[j], htmlField.val());
                }
            }
        }
    }

    return text;
}

function setFieldValues(values) {
    for (var i = 0; i < values.length; i++) {
        if (values[i].target != undefined && values[i].dataField != undefined)
            $("[name=" + values[i].target + "]").val($("[name=" + values[i].dataField + "]").val()).trigger("change");
        else if (values[i].target != undefined && values[i].value != undefined)
            $("[name=" + values[i].target + "]").val(values[i].value).trigger("change");
    }
}

function ValidateCondition(validation) {
    var result = true;
    var delimiters = [" #and# ", " #or# "];

    var splitMultiple = function (str, delimiters) {
        delimiters.forEach(function (item) { str = str.split(item).join(":;") });
        return str.split(":;");
    }

    var strValid = validation;
    var pieces = splitMultiple(strValid, delimiters);

    var operatr = "";
    for (var i = 0; i < pieces.length; i++) {
        var condition = pieces[i];
        if (!(pieces[0] == condition))
            operatr = validation.substring(validation.indexOf(condition) - 7, validation.indexOf(condition)) == " #and# " ? "and" : "or";

        if (condition.indexOf("NotIn(") > -1 || condition.indexOf("In(") > -1) {
            var notInFunc = "NotIn(";
            var inFunc = "In(";
            var funct = condition.indexOf(notInFunc) > -1 ? notInFunc : inFunc;
            var clearParameter = condition.substring(funct.length, condition.length - 1);
            arr = clearParameter.split(',');
            
            var parameters = [];
            parameters = arr.splice(0, 1);
            parameters.push(arr.join(','));

            var validateIn = ParseFieldsToFormData(parameters[0].trim()).toLowerCase();
            var list = parameters[1].trim().substring(1, parameters[1].trim().length - 1).split('|');
            list = list.map(function (x) { return x.toLowerCase() });

            var operation = funct == inFunc ? list.indexOf(validateIn) > -1 : !(list.indexOf(validateIn) > -1);

            if (operatr == "and") result = result && operation;
            else if (operatr == "or") result = result || operation;
            else result = operation;
        }
        else if (condition.indexOf("!=") > -1 || condition.indexOf("==") > -1)
        {
            var isEqual = condition.indexOf("==") > -1;
            var equalities = [ "==", "!=" ];
            var constants = splitMultiple(condition, equalities);

            var value1 = ParseFieldsToFormData(constants[0].trim().replace(/'/g, "").replace(/"/g, ""));
            var value2 = ParseFieldsToFormData(constants[1].trim().replace(/'/g, "").replace(/"/g, ""));

            var comparisonResult = isEqual ? value1 == value2 : value1 != value2;
            if (operatr == "and") result = result && comparisonResult;
            else if (operatr == "or") result = result || comparisonResult;
            else result = comparisonResult;
        }
    }

    return result;
}

function SetFieldsValidations(validation, ctrlId, inputType) {
    var dom = (new DOMParser).parseFromString('<!doctype html><body>' + validation, 'text/html');
    var decodedString = dom.body.textContent;

    var display = ValidateCondition(decodedString);
    var hideVisibility = false;

    var input;
    if (inputType == "action") {
        input = $("#action" + ctrlId);
    }
    else if (inputType == "field") {
        ctrl = $("[name='" + ctrlId + "']");
        if (ctrl.parent().hasClass("full-row")) {
            input = ctrl.parents(".row").first();
        }
        else {
            hideVisibility = true;
            input = ctrl.parents(".halfrow").first();
        }
    }

    if (hideVisibility)
        display ? input.css('visibility', 'visible') : input.css('visibility', 'hidden');
    else
        display ? input.show() : input.hide();
}

function AddItemToList(list, value, addUserNDate) {
    var ctrlList = $("[name^='" + list + "']").last();
    var ctrlListId = ctrlList.attr("id");
    var comment = value;

    var idSplit = ctrlList.parents("[class^=col]").find("input[type=hidden]").last().attr("id").split(":");
    var lastFileIndex = parseInt(idSplit[1]);
    list += ":" + lastFileIndex;

    var container = $("[id='container" + list + "']").clone();
    var select = "[name$=':" + lastFileIndex + "']";

    container.find(select).addBack(select).each(function (i, ctrl) {
        if ($(ctrl).attr("id") != undefined)
            $(ctrl).attr("id", $(ctrl).attr("id").replace(":" + lastFileIndex, ":" + (lastFileIndex + 1)));

        if ($(ctrl).attr("name") != undefined)
            $(ctrl).attr("name", $(ctrl).attr("name").replace(":" + lastFileIndex, ":" + (lastFileIndex + 1)));

        if ($(ctrl).attr("onclick") != undefined)
            $(ctrl).attr("onclick", $(ctrl).attr("onclick").replace(":" + lastFileIndex, ":" + (lastFileIndex + 1)));
    });

    container.find("input[type=hidden]").val(null);
    container.insertAfter("[id='container" + ctrlListId + "']");

    if (addUserNDate) comment += " (" + $("[name=currentUser]").val() + ": " + (new Date()).toLocaleString('es-CL') + ")";

    $("input[id='" + ctrlListId + "']").val(comment);
    $("div.input-list[name='" + ctrlListId + "']").html(comment);
    $("div[name^='" + idSplit[0] + ":']").removeClass("last");
    $("div[id='container" + ctrlListId + "'] div").addClass("last");
    $("[id='container" + ctrlListId + "']").show();
}

/* start checkbox/radiobutton show/hide/remove fields */
function removeColumns(control, hide) {
    var colContainer = $("[name=" + control + "]").parents("div.full-row");
    //var parentSelector = colContainer.length > 0 ? "div.full-row" : "div.half-row";
    var parentSelector = colContainer.length > 0 ? "div.col-sm-12" : "div.col-sm-6";

    var container = $("[name=" + control + "]").parents(parentSelector);
    var state = hide ? container.hide() : container.show();

    var prev = $("[name=" + control + "]").parents("div.halfrow").prev(".halfrow.empty");
    var next = $("[name=" + control + "]").parents("div.halfrow").next(".halfrow.empty");

    if (prev.length > 0 || next.length > 0)
        var state = hide ? container.parent(".row").hide() : container.parent(".row").show();

    var inputs = container.find("[type='radio']:checked:visible,[type='checkbox']:checked:visible");
    if (inputs.length > 0)
        inputs.each(function (i, item) {
            var value = $(item).is(":checked");

            $(item).click();
            if (value) $(item).prop("checked", true);
        });
}

function radiocheckClick(controlsToHide, hideShowControls, funct) {
    var ctrlsToHideSplit = controlsToHide.split(",");
    ctrlsToHideSplit.forEach(function (controlToHide) {
        funct(controlToHide, true);

        for (var i = 0; i < hideShowControls.length; i++) {
            if (ctrlsToHideSplit.indexOf(hideShowControls[i]) == -1)
                funct(hideShowControls[i], false);
        }
    });
}

function getHideShowControls(controlGroup, attribute) {
    var controls = [];
    controlGroup.each(function (index, item) {
        if ($(item).attr(attribute) != undefined)
            $(item).attr(attribute).split(",").forEach(function (controlToHide) {
                if (controls.indexOf(controlToHide) == -1) controls.push(controlToHide);
            });
    });

    return controls;
}
/* end checkbox/radiobutton show/hide/remove fields */

function updateCustomFullValue(ctrl) {
    var parent = ctrl.parents(".table.custom-field").first();
    var fullText = parent.find("input[type!=hidden]").map(function () {
        return $(this).val();
    }).get().join('-');

    parent.find("input[type=hidden]").val(fullText);
}

function addItemToCustomList(ctrl) {
    //var ctrl = $(".custom-list-field .add-item a");
    var container = ctrl.parents(".custom-list-field").first();
    var lastItem = container.find("tr").last();
    var lastItemIdSplit = lastItem.attr("id").split("Row:");

    var field = lastItemIdSplit[0];
    var lastIndex = parseInt(lastItemIdSplit[1]);
    var newItem = lastItem.clone();

    var select = "[id$=':" + lastIndex + "']";
    newItem.find(select).addBack(select).each(function (i, element) {
        $(element).attr("id", $(element).attr("id").replace(":" + lastIndex, ":" + (lastIndex + 1)));

        if ($(element).attr("name") != undefined)
            $(element).attr("name", $(element).attr("name").replace(":" + lastIndex, ":" + (lastIndex + 1)));

        if ($(element).attr("onclick") != undefined)
            $(element).attr("onclick", $(element).attr("onclick").replace(":" + lastIndex, ":" + (lastIndex + 1)));
    });

    newItem.find("input[type=textbox]").val(null);
    newItem.insertAfter(lastItem);
}

function populate(frm, data) {
    var execHideShowFields = function (ctrl) {
        if (ctrl.attr("data-hidden") != undefined) {
            var controls = getHideShowControls($("[name=" + ctrl.attr("name") + "]"), "data-hidden");
            radiocheckClick(ctrl.attr("data-hidden"), controls, removeColumns);
        }
    }

    var indexCorrected = [];
    $.each(data, function (key, value) {
        var ctrl = $("input[name='" + key + "']", frm);

        if (key.indexOf(":") > -1 && ctrl.length == 0) {
            var index = parseInt(key.split(":")[1]);

            if (index > 0) {
                var firstItemId = key.replace(":" + index, ":" + 0);
                var firstItem = $("input[name='" + firstItemId + "']", frm);
                var correctedIndex = indexCorrected.filter(function (el) { return el.parent == firstItem.parents(".custom-list-field").attr("id") && el.oldIndex == index; });

                if (correctedIndex.length > 0)
                    key = key.replace(":" + index, ":" + correctedIndex[0].newIndex)
                else {
                    var newItemIndex = parseInt(firstItem.parents(".custom-list-field").find("tr").last().attr("id").split(":")[1]) + 1;

                    if (firstItem.parents(".custom-list-field").length > 0) {
                        addItemToCustomList(firstItem.parents(".custom-list-field").find("tr").last());

                        var parent = firstItem.parents(".custom-list-field").attr("id");

                        indexCorrected.push({ parent: parent, oldIndex: index, newIndex: newItemIndex });
                        key = key.replace(":" + index, ":" + newItemIndex)
                    }
                }

                ctrl = $("input[name='" + key + "']", frm);
            }
        }

        switch (ctrl.attr("type")) {
            case "checkbox":
                ctrl.attr("checked", false);
                ctrl.each(function () {
                    if (value.indexOf($(this).attr('value')) > -1 && $(this).attr('type') != "hidden") {
                        $(this).attr("checked", true);
                        $(this).next("input[type=hidden]").val($(this).attr('value'));

                        execHideShowFields($(this));
                    }
                });

                break
            case "radio":
                ctrl.each(function () {
                    if ($(this).attr('value') == value && $(this).attr('type') != "hidden") {
                        $(this).attr("checked", value);
                        $(this).next("input[type=hidden]").val($(this).attr('value'));

                        execHideShowFields($(this));
                    }
                });
                break;
            case "hidden":
                if (ctrl.attr("id") != undefined && ctrl.attr("id").indexOf(":") > -1 && value != "") {
                    var field = ctrl.attr("id").split(":")[0];
                    AddItemToList(field, value);
                }
                else ctrl.val(value).trigger("change");;
            default:
                ctrl.val(value).trigger("change");;
                if (ctrl.parents(".table.custom-field").length > 0) {
                    updateCustomFullValue(ctrl)
                }
        }

        //if (ctrl.is("[data-hidden]")) {
        //    ctrl.each(function (index, item) {
        //        if ($(item).is("[data-hidden]:checked")) {
        //            var controlsToHide = $(item).attr("data-hidden");
        //            controlsToHide.split(",").forEach(function (controlToHide) { showHideColumns(controlToHide, true); });
        //        }
        //    });
        //}
    });
}

jQuery.fn.filterByText = function (textbox) {
    return this.each(function () {
        var select = this;
        var options = [];
        $(select).find('option').each(function () {
            var option = {
                value: $(this).val(),
                text: $(this).text()
            };

            var attributes = [];
            var attributesList = $(this)[0].attributes;
            //var attributesList = $(this).attributes$("#selectUserNameList option").first()[0].attributes;
            for (var i = 0; i < attributesList.length; i++) {
                if (attributesList[i].name != "value") {
                    var attribute = {
                        name: attributesList[i].name,
                        value: attributesList[i].value
                    }
                    attributes.push(attribute);
                }
            }
            option["attributes"] = attributes;
            options.push(option);
        });
        $(select).data('options', options);

        $(textbox).bind('change keyup', function () {
            var options = $(select).empty().data('options');
            var search = $.trim($(this).val());
            var regex = new RegExp(search, "gi");

            $.each(options, function (i) {
                var option = options[i];
                if (option.text.match(regex) !== null) {
                    var optionCtrl = $('<option>').text(option.text).val(option.value);
                    if (option.attributes != undefined && option.attributes.length > 0) {
                        option.attributes.forEach(function (item) {
                            optionCtrl.attr(item.name, item.value);
                        });
                    }

                    $(select).append(optionCtrl);
                }
            });
        });
    });
};

jQuery.extend(jQuery.expr[':'], {
    attrStartsWith: function (el, _, b) {
        for (var i = 0, atts = el.attributes, n = atts.length; i < n; i++) {
            if (atts[i].nodeName.toLowerCase().indexOf(b[3].toLowerCase()) === 0) {
                return true;
            }
        }

        return false;
    }
});