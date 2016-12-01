$(function () {
    $("#FromDateTime")
        .datetimepicker({
            //minDate: moment().add(-60, 'days'),
            maxDate: moment(), // Current day
        });
    $("#ToDateTime")
        .datetimepicker({
            useCurrent: false, //Important! See issue #1075
            //minDate: moment().add(-60, 'days'),
            maxDate: moment(), // Current day
        });
    $("#FromDateTime")
        .on("dp.change",
            function (e) {
                $("#ToDateTime").data("DateTimePicker").minDate(e.date);
            });
    $("#ToDateTime")
        .on("dp.change",
            function (e) {
                $("#FromDateTime").data("DateTimePicker").maxDate(e.date);
            });

    $("#faultDetail")
        .click(function (event) {
            event.preventDefault();
        });


    $(".btn-Resolved").attr("disabled", "disabled");
    $(".btn-Flagged").attr("disabled", "disabled");
    $(".btn-Resubmitted").attr("disabled", "disabled");

    var currentStatus = $("#hCurrentStatus").val();
    //unresolved or flagged
    if (currentStatus === "UnResolved" || currentStatus === "Flagged") {
        $("#chkHeader")
            .change(function () {
                if (this.checked) {
                    $(".chkRow").prop("checked", true);
                } else {
                    $(".chkRow").prop("checked", false);
                }
            });
    } else {
        $(".chkRow").attr("disabled", true);
        $("#chkHeader").attr("disabled", true);
    }

});

function displayfaultDetail(faultId) {
    var url = $("#fltDtlUrl").val();
    $("#faultDetailModal").html("<h3>Loading....</h3>");
    $("#faultDetailModal").modal("show");
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { faultid: faultId },
        success: function (data, textStatus, jqXHR) {
            $("#faultDetailModal").html(data);
            $("#faultDetailModal").modal("show");
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('unexpected error in "displayfaultDetail" method.');
            waitingDialog.hide();
        }
    });
}

function resolveFault(faultId, actionId, btnId) {
    var url = $("#fltUpdStatusUrl").val();
    $("#confirmActionModal")
        .modal({ backdrop: "static", keyboard: false })
        .on("click",
            "#btnProceed",
            function (e) {

                var comment = $("#comment").val();

                if (comment.length === 0) {
                    alert("Please provide a comment.");
                    return false;
                }

                $.ajax({
                    url: url,
                    method: "POST",
                    dataType: "json",
                    cache: false,
                    data: { statusType: actionId, faultIds: faultId, comment: comment },
                    success: function (data, textStatus, jqXHR) {
                        if (data.Success === true) {
                            $("#divAlert").addClass("alert").addClass("alert-success").html(data.Data);
                            $("#" + btnId).attr("disabled", "disabled")
                        } else {
                            $("#divAlert").addClass("alert").addClass("alert-danger").html(data.Error);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('unexpected error in "displayfaultDetail" method.');
                    }
                });
            });


}

function bulkResolveFault(actionId) {
    var url = $("#fltUpdStatusUrl").val();
    $("#confirmActionModal")
        .modal({ backdrop: "static", keyboard: false })
        .on("click",
            "#btnProceed",
            function (e) {

                var comment = $("#comment").val();

                if (comment.length === 0) {
                    alert("Please provide a comment.");
                    return false;
                }

                var faultIds = new Array();;
                $(".chkRow")
                    .each(function (index) {
                        console.log(index + ": " + $(this).text());
                        if ($(this).prop("checked")) {
                            faultIds.push($(this).data("value").trim());
                        }
                    });

                if (faultIds.length === 0) {
                    $("#divAlert")
                        .addClass("alert")
                        .addClass("alert-danger")
                        .html("Please select one or more rows to bulk resolve/flag.");
                    return false;
                }

                $.ajax({
                    url: url,
                    method: "POST",
                    dataType: "json",
                    cache: false,
                    data: { statusType: actionId, faultIds: faultIds, comment: comment },
                    success: function (data, textStatus, jqXHR) {
                        if (data.Success === true) {
                            $("#divAlert")
                                .addClass("alert")
                                .removeClass("alert-danger")
                                .addClass("alert-success")
                                .html(data.Data);
                            //window.location.reload();
                        } else {
                            $("#divAlert")
                                .addClass("alert")
                                .removeClass("alert-success")
                                .addClass("alert-danger")
                                .html(data.Error);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert("unexpected error in method.");
                    }
                });
            });
}

function displayMsgDetail(faultId, messageId) {
    var url = $("#msgDtlUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { faultId: faultId, messageId: messageId },
        success: function (data, textStatus, jqXHR) {
            $("#msgDtls").html(data).show();
            $("#msgDtlTable").hide();
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('unexpected error in "displayMsgDetail" method.');
            waitingDialog.hide();
        }
    });
}

function displayMessageList() {
    $("#msgDtls").html("").hide();
    $("#msgDtlTable").show();
}

function createAlert() {
    var url = $("#createAlertUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        success: function (data, textStatus, jqXHR) {
            $("#createAlertModal").html(data);
            $("#createAlertModal").modal("show");
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error occurred.");
            waitingDialog.hide();
        }
    });
}

function editAlert(alertId) {
    var url = $("#editAlertUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "alertId": alertId },
        success: function (data, textStatus, jqXHR) {
            $("#createAlertModal").html(data);
            $("#createAlertModal").modal("show");
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error occurred.");
            waitingDialog.hide();
        }
    });
}

function deleteAlertSubscription(alertSubscriptionId) {
    var url = $("#deleteSubsUrl").val();
    $("#confirmActionModal")
        .modal({ backdrop: "static", keyboard: false })
        .on("click",
            "#btnProceed",
            function (e) {
                waitingDialog.show();
                $.ajax({
                    url: url,
                    method: "POST",
                    data: { "alertSubscriptionId": alertSubscriptionId },
                    success: function (data, textStatus, jqXHR) {
                        if (data.Success === true) {
                            $("#divAlert")
                                .removeClass("alert-danger")
                                .addClass("alert")
                                .addClass("alert-success")
                                .html(data.Data);
                            window.location.reload();
                        } else {
                            $("#divAlert")
                                .removeClass("alert-success")
                                .addClass("alert")
                                .addClass("alert-danger")
                                .html(data.Error);
                        }

                        waitingDialog.hide();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert("unexpected error occurred.");
                        waitingDialog.hide();
                    }
                });
            });
}

function deleteAlert(alertId) {
    $("#confirmActionModal")
        .modal({ backdrop: "static", keyboard: false })
        .on("click",
            "#btnProceed",
            function (e) {
                var url = $("#deleteAlertUrl").val();
                waitingDialog.show();

                $.ajax({
                    cache: false,
                    url: url,
                    data: { "alertId": alertId },
                    method: "POST",
                    success: function (data, textStatus, jqXHR) {
                        if (data.Success === true) {
                            window.location.reload();

                        } else {
                            waitingDialog.hide();
                            alert(data.Error);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert("unexpected error .");
                    }
                });
            });
}

function toggleAlertSubscription(alertSubscriptionId, currentState) {
    var url = $("#toggleSubsUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        data: { "alertSubscriptionId": alertSubscriptionId, "currentState": currentState },
        success: function (data, textStatus, jqXHR) {
            if (data.Success === true) {
                $("#divAlert").removeClass("alert-danger").addClass("alert").addClass("alert-success").html(data.Data);
            } else {
                $("#divAlert").removeClass("alert-success").addClass("alert").addClass("alert-danger").html(data.Error);
            }

            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error occurred.");
            waitingDialog.hide();
        }
    });
}

function subscribe(alertId) {
    var url = $("#subsribeUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { alertId: alertId },
        success: function (data, textStatus, jqXHR) {
            $("#createAlertModal").html(data);
            $("#createAlertModal").modal("show");
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error occurred.");
            waitingDialog.hide();
        }
    });
}