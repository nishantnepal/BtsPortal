$(function () {
    var currentStatus = $("#hCurrentStatus").val();
    //unresolved or flagged
    if (currentStatus === "SuspendedResumable" || currentStatus === "SuspendedNonResumable") {
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
function reloadArtifacts() {
    var appName = $("#appList option:selected").text();
    var artifactType = $("#artifactTypeList option:selected").text();
    $("#Request_ArtifactId").empty().append('<option>Loading..</option>');
    var url = $("#artPerApp").val();
    $.ajax({
        url: url,
        data: { appName: appName, artifactType: artifactType },
        success: function (response, textStatus, jqXHR) {
            $("#Request_ArtifactId").empty().append('<option>--All-</option>');
            $.each(response, function (i, data) {
                var option = $('<option></option>').attr("value", data.Value).text(data.Key);
                $("#Request_ArtifactId").append(option);
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('unexpected error.');
        }
    });

}

function displayInstanceDetail(instanceId) {
    var url = $("#instDtlUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        method: "POST",
        data: { instanceId: instanceId },
        success: function (data, textStatus, jqXHR) {
            $('#detailModal').html(data);
            $('#detailModal').modal('show');
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('unexpected error.');
            waitingDialog.hide();
        }
    });
}

function displayMsgDetail(instanceId, messageId) {
    var url = $("#msgDtlUrl").val();
    waitingDialog.show();
    $.ajax({
        url: url,
        data: { instanceId: instanceId, messageId: messageId },
        success: function (data, textStatus, jqXHR) {
            $('#msgDtls').html(data).show();
            $('#msgDtlTable').hide();
            waitingDialog.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('unexpected error.');
            waitingDialog.hide();
        }
    });
}

function displayMessageList() {
    $('#msgDtls').html('').hide();
    $('#msgDtlTable').show();
}

function migrateInstance(instanceId, btnId) {
    var url = $("#migrateInstUrl").val();
    $("#divAlert").removeClass("alert")
                .removeClass("alert-danger")
                .removeClass("alert-success")
                .html("");
    $("#confirmActionModal")
        .modal({ backdrop: "static", keyboard: false })
        .on("click",
            "#btnProceed",
            function (e) {
                waitingDialog.show("Migrating..");
                $.ajax({
                    url: url,
                    method: "POST",
                    dataType: "json",
                    cache: false,
                    data: { instanceIds: instanceId},
                    success: function (data, textStatus, jqXHR) {
                        if (data.Success === true) {
                            $("#divAlert").addClass("alert").addClass("alert-success").html(data.Data);
                            $("#" + btnId).attr("disabled", "disabled");
                        } else {
                            $("#divAlert").addClass("alert").addClass("alert-danger").html(data.Error);
                        }
                        waitingDialog.hide();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('unexpected error method.');
                        waitingDialog.hide();
                    }
                });
            });


}

function bulkMigrateInstances() {
    var url = $("#migrateInstUrl").val();
    $("#divAlert").removeClass("alert")
                .removeClass("alert-danger")
                .removeClass("alert-success")
                .html("");

    $("#confirmActionModal")
        .modal({ backdrop: "static", keyboard: false })
        .on("click",
            "#btnProceed",
            function (e) {
                var instanceIds = new Array();;
                $(".chkRow")
                    .each(function (index) {
                        if ($(this).prop("checked")) {
                            instanceIds.push($(this).data("value").trim());
                        }
                    });

                if (instanceIds.length === 0) {
                    $("#divAlert")
                        .addClass("alert")
                        .addClass("alert-danger")
                        .html("Please select one or more rows to migrate to esb.");
                    return false;
                }

                waitingDialog.show("Migrating..");
                $.ajax({
                    url: url,
                    method: "POST",
                    dataType: "json",
                    cache: false,
                    data: { instanceIds: instanceIds},
                    success: function (data, textStatus, jqXHR) {
                        if (data.Success === true) {
                            $("#divAlert")
                                .addClass("alert")
                                .removeClass("alert-danger")
                                .addClass("alert-success")
                                .html(data.Data);
                            //window.location.reload();
                            waitingDialog.hide();
                        } else {
                            $("#divAlert")
                                .addClass("alert")
                                .removeClass("alert-success")
                                .addClass("alert-danger")
                                .html(data.Error);
                            waitingDialog.hide();
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert("unexpected error in method.");
                        waitingDialog.hide();
                    }
                });
            });
}
