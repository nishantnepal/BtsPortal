$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

function loadActivityView() {
    var activityName = $("#activity").val();
    var url = $("#activity").data("url");
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        data: { activityName: activityName },
        success: function (data, textStatus, jqXHR) {
            $("#vwPort").html(data);
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('unexpected error.');
            waitingDialog.hide();
        }
    });
}

function editConfiguration() {
    var viewId = $("#view").val();
    if (viewId !== '' && viewId !== null) {
        loadViewConfiguration(viewId);
    }
}

function loadViewConfiguration(id) {
    var url = $("#confUrl").val();
    var activity = $("#activity").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "id": id, "activityName": activity },
        method: "GET",
        success: function (data, textStatus, jqXHR) {
            $("#bamModal").html(data);
            $("#bamModal").modal("show");
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function toggleConfiguration(viewId, currentState) {
    var url = $("#toggleConfUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        data: { "viewId": viewId, "currentState": currentState },
        success: function (data, textStatus, jqXHR) {
            if (data.Success === true) {
                loadActivityView();
            } else {
                alert(data.Error);
            }

            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error occurred.");
            waitingDialog.hide();
        }
    });
}


