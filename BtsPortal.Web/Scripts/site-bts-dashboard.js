$(function () {
    $(".btsPorts").tooltip({ html: true });
});

function markActive(id) {
    $(".list-group-item").removeClass("active");
    $("#" + id).addClass("active");
    $("#divStatus").html("").removeClass("alert");

}

function loadSummary() {
    var url = $("#summary").data("url");
    markActive("summary");
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            $("#btsVwPort").html(data);
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function loadHosts() {
    var url = $("#hosts").data("url");
    markActive("hosts");
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            $("#btsVwPort").html(data);
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function loadApplications() {
    var url = $("#application").data("url");
    markActive("application");
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            $("#btsVwPort").html(data);
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function loadApplicationArtifactSummary(appId, statusId) {
    var url = $("#appArtSummUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "appId": appId, "status": statusId },
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            $("#detailModal").html(data);
            $("#detailModal").modal("show");
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function hostOperation(action, hostName) {
    var url = $("#hostOpUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "action": action, "hostName": hostName },
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            if (data.Success === true) {
                //$("#divStatus")
                //    .removeClass("alert-danger")
                //    .addClass("alert")
                //    .addClass("alert-success")
                //    .html(data.Data);
                loadHosts();
            } else {
                $("#divStatus")
                    .removeClass("alert-success")
                    .addClass("alert")
                    .addClass("alert-danger")
                    .html(data.Error);
            }
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function loadApplicationStatus(activeTab) {
    var applicationName = $("#appList").val();
    markActive("application");
    var url = $("#appList").data("url");
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        data: { applicationName: applicationName },
        success: function (data, textStatus, jqXHR) {
            $("#appData").html(data);
            $('#artifactsTab a[href="' + activeTab + '"').tab("show");
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function orchOperation(action, appName, orchName) {
    var url = $("#orchOpUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "action": action, "appName": appName, "orchName": orchName },
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            if (data.Success === true) {
                //$("#divStatus")
                //    .removeClass("alert-danger")
                //    .addClass("alert")
                //    .addClass("alert-success")
                //    .html(data.Data);
                loadApplicationStatus("#orch");
            } else {
                $("#divStatus")
                    .removeClass("alert-success")
                    .addClass("alert")
                    .addClass("alert-danger")
                    .html(data.Error);
            }
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function sendPortOperation(action, appName, portName) {
    var url = $("#spOpUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "action": action, "appName": appName, "portName": portName },
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            if (data.Success === true) {
                loadApplicationStatus("#sendPorts");

            } else {
                $("#divStatus")
                    .removeClass("alert-success")
                    .addClass("alert")
                    .addClass("alert-danger")
                    .html(data.Error);
            }
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function receiveLocationOperation(action, appName, portName, locationName) {
    var url = $("#rpOpUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "action": action, "appName": appName, "portName": portName, "locationName": locationName },
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            if (data.Success === true) {
                loadApplicationStatus("#recPorts");

            } else {
                $("#divStatus")
                    .removeClass("alert-success")
                    .addClass("alert")
                    .addClass("alert-danger")
                    .html(data.Error);
            }
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function loadSso(selectedApp) {
    var url = $("#sso").data("url");
    markActive("sso");
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "selectedApp": selectedApp },
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            $("#btsVwPort").html(data);
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function loadSsoData() {
    var appName = $("#ssoAppList").val();
    markActive("sso");
    var url = $("#ssoAppList").data("url");
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        data: { appName: appName },
        success: function (data, textStatus, jqXHR) {
            $("#appData").html(data);
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function updateSsoSetting(appName, isBtdf, key, valueId) {
    var value = $("#" + valueId).val();
    var orgValue = $("#" + valueId).attr("data-orgValue");
    saveSsoSetting(appName, isBtdf, key, value, orgValue, valueId);

}

function saveSsoSetting(appName, isBtdf, key, value, orgValue, valueId) {
    var url = $("#ssoOpUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { "appName": appName, "orgValue": orgValue, "key": key, "value": value, "isBtdf": isBtdf },
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            if (data.Success === true) {
                if (valueId === null) {
                    loadSsoData();
                } else {
                    $("#status" + valueId).text("Saved");
                }
            } else {
                $("#divStatus")
                    .removeClass("alert-success")
                    .addClass("alert")
                    .addClass("alert-danger")
                    .html(data.Error);
            }
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });

}

function createSsoProperty() {
    $("#appName").val($("#ssoAppList").val());
    $("#value1").val("");
    $("#key1").val("");
    $("#newDataModal")
        .modal({ backdrop: "static", keyboard: false })
        .on("click",
            "#btnCreate",
            function (e) {
                var key = $("#key1").val();
                if (key.length === 0) {
                    alert("Please provide a valid key name.");
                    return false;
                }
                var value = $("#value1").val();
                if (value.length === 0) {
                    alert("Please provide a valid value.");
                    return false;
                }
                var appName = $("#ssoAppList").val();
                saveSsoSetting(appName, "false", key, value, null, null);

            });
}

function loadEdiParties() {
    var url = $("#edi").data("url");
    markActive("edi");
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        success: function (data, textStatus, jqXHR) {
            $("#btsVwPort").html(data);
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function createNewSsoApp() {
    $("#newAppModal")
        .modal({ backdrop: "static", keyboard: false })
        .on("click",
            "#btnCreateNewApp",
            function (e) {
                var appName = $("#newAppName").val();
                if (appName.length === 0) {
                    alert("Please provide a valid application name.");
                    return false;
                }
                var url = "/portal/Bts/BtsOps/CreateNewSsoApp";
                waitingDialog.show();
                $.ajax({
                    url: url,
                    data: { "appName": appName },
                    success: function (data, textStatus, jqXHR) {
                        if (data.Success === true) {
                            loadSso(appName);
                        } else {
                            $("#divStatus")
                                .removeClass("alert-success")
                                .addClass("alert")
                                .addClass("alert-danger")
                                .html(data.Error);
                        }
                        waitingDialog.hide();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert("unexpected error.");
                        waitingDialog.hide();
                    }
                });
            });
}

function loadEdiPartyData2() {
    markActive("edi");
    var value = $("#ddlParty").selectpicker("val");
    loadEdiPartyData(value);
}

function loadEdiPartyData(partyName) {
    var url = $("#ddlParty").data("url");
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        data: { partyName: partyName },
        success: function (data, textStatus, jqXHR) {
            $("#appData").html(data);
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
            waitingDialog.hide();
        }
    });
}

function showBatch(selectId) {
    var selectedBatch = $("#" + selectId).val();
    $(".batches").hide();
    $("#batch" + selectedBatch).show();
}

function markActive2(id) {
    $(".list-group-item-sm").removeClass("active");
    $("#" + id).addClass("active");

}

function batchStatusAction(action, batchId, batchName, agreementName, sendingPartner, receivingPartner, oneWayAgreementId) {
    var url = $("#ediBatchStatusOpUrl").val();
    if (action === "Start" || action === "Stop" || action === "Override") {
        $(".batch" + batchId).prop("disabled", true);
    }

    $("#divStatus2").html("Loading...");

    $.ajax({
        url: url,
        data: { action: action, batchId: batchId, batchName: batchName, agreementName: agreementName, sendingPartner: sendingPartner, receivingPartner: receivingPartner, oneWayAgreementId: oneWayAgreementId },
        method: "POST",
        success: function (response, textStatus, jqXHR) {
            if (response.Success === true) {
                $("#divStatus2").html("");
                $("#divStatus2").html("Successfully submitted action.");
                if (action === "Start" || action === "Stop" || action === "Override") {
                    $("#btnRefresh" + batchId).removeAttr("disabled");
                }
                if (response.Data.Status === 1) {
                    $("#batchStatus" + batchId)
                        .html('<i class="fa fa-check-circle fa-2" style="color: yellow"></i>').attr("title", "PendingStart");
                }
                else if (response.Data.Status === 2) {
                    $("#batchStatus" + batchId)
                        .html('<i class="fa fa-exclamation-circle fa-2" aria-hidden="true" style="color: red"></i>').attr("title", "Stopped");
                    $("#btnStart" + batchId).removeAttr("disabled");
                }
                else if (response.Data.Status === 3) {
                    $("#batchStatus" + batchId)
                        .html('<i class="fa fa-check-circle fa-2" style="color: green"></i>').attr("title", "Started");
                    $("#btnStop" + batchId).removeAttr("disabled");
                    $("#btnOverride" + batchId).removeAttr("disabled");

                }
                else if (response.Data.Status === 4) {
                    $("#batchStatus" + batchId)
                        .html('<i class="fa fa-exclamation-circle fa-2" style="color: yellow"></i>').attr("title", "Waiting for control message to be processed");

                }

            } else {
                $("#divStatus2").html(response.Error);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("unexpected error.");
        }
    });

}

