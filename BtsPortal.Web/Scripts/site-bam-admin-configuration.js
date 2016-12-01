function updateConfig(e) {
    e.preventDefault();
    $("#alert").removeClass("alert").removeClass("alert-danger").html("");
    var sqlNoOfRows = $("#sqlNoOfRows").val();
    var sqlToExecute = $("#sql").val();
    var viewName = $("#name").val();
    var activityName = $("#activity").val();
    var id = $("#vId").val();
    var sqlOrderBy = $("#sqlOrderBy").val();
    var noOfRowsPerPage = $("#noOfRowsPerPage").val();
    var currentFiltersCleared = $("#currentFiltersCleared").val();
    var proceed = true;
    if (viewName === '') {
        $("#alert").addClass("alert").addClass("alert-danger").html('Invalid View Name');
        proceed = false;
    }
    if (sqlToExecute === '') {
        $("#alert").addClass("alert").addClass("alert-danger").html('Invalid Sql Statement');
        proceed = false;
    }
    if (proceed) {
        var url = $("#btnUpdateConfig").data("url");
        $.ajax({
            url: url,
            method: "POST",
            data: { activityName: activityName, sqlNoOfRows: sqlNoOfRows, sqlToExecute: sqlToExecute, viewName: viewName, id: id, sqlOrderBy: sqlOrderBy, noOfRowsPerPage: noOfRowsPerPage, viewFilterParameters: selectedConditions, currentFiltersCleared: currentFiltersCleared },
            success: function (data, textStatus, jqXHR) {
                if (data.Success === true) {
                    loadActivityView(data.Data);
                    $("#notif").addClass("alert").addClass("alert-success").removeClass("alert-danger").html('View Created Successfully');
                    $("#bamModal").modal("hide");
                } else {
                    $("#alert").addClass("alert").addClass("alert-danger").html(data.Error);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('unexpected error.');
            }
        });
    }
}

function addCondition(event) {
    event.preventDefault();

    var paramName = $('#parameter').val();
    var paramType = $('#type').val();
    var displayName = $('#displayName').val();
    var counter = selectedConditions.length + 1;
    var data = { "Name": paramName, "DisplayName": displayName, "ParameterType": paramType };

    if ($.inArray(data, selectedConditions) === -1) {
        selectedConditions.push(data);
        $('#tblSelectedConditions')
            .append(
                "<tr id='dat_" +
                counter +
                "'>" +
                "<td>" +
                paramName +
                "</td>" +
                "<td>" +
                paramType +
                "</td>" +
                "<td>" +
                displayName +
                "</td>" +
                "<td><button class='btn btn-danger btn-xs' onclick='removeSelectedCondition(" + counter + ")'><i class='fa fa-times' aria-hidden='true'></i></button></td>" +
                "</tr>"
            );
    }

    $('#parameter').val('');
    $('#displayName').val('');
}

function removeSelectedCondition(counter) {
    selectedConditions.splice(counter, 1);
    $('#dat_' + counter).remove();
}

function removeAllParameters(event) {
    event.preventDefault();
    $("#tblSelectedConditions").find("tr:gt(1)").remove();
    $("#currentFiltersCleared").val("true");
    $("#filterRow").show();
}