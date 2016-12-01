$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    loadViewData();
});



function loadActivityView(selectedId) {
    var activityName = $("#activity").val();
    if (activityName !== '') {
        $("#btnCreate").removeAttr("disabled");
    }
    var url = $("#activity").data("url");
    $.ajax({
        url: url,
        data: { activityName: activityName },
        success: function (response, textStatus, jqXHR) {
            $("#view").empty().append("<option value=''>--Select One-</option>");
            $.each(response, function (i, data) {
                var option = $('<option></option>').attr("value", data.Value).text(data.Key);
                $("#view").append(option);
            });

            if (selectedId !== '') {
                $("#view").val(selectedId);
                loadViewData('1',true);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('unexpected error.');
        }
    });
}

function loadViewData(page, didViewChange) {
    $("#notif").removeClass("alert")
               .removeClass("alert-danger")
               .html("");

    var filterData = [];
    var viewId = $("#view").val();
    var proceed = true;
    var errors = "";
    $(".numeric").each(function () {
        var id = $(this).attr('id');
        var value = $(this).val();
        var displayName = $("#sp_" + id).text();
        if ($.isNumeric(value) === false && value !== "") {
            errors = errors + "Invalid numeric value '" + value + "' for '" + displayName + "'. ";
            proceed = false;
        }
    });

    if (errors !== "") {
        $("#notif")
               .addClass("alert")
               .addClass('alert-danger')
               .html(errors);
    }

    if (proceed) {
        if (didViewChange === "false") {
            $(".form-input").each(function () {
                var id = $(this).attr('id');
                var value = $(this).val();
                var data = { "Key": id, "Value": value };
                filterData.push(data);
            });
        }

        var url = $("#view").data("url");
        if (viewId !== '' && viewId !== null) {
            $("#vwPort").html("Loading....");
            $.ajax({
                url: url,
                method: "POST",
                data: { viewId: viewId, page: page, filterData: filterData },
                success: function (data, textStatus, jqXHR) {
                    $("#vwPort").html(data);

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('unexpected error.');
                }
            });
        }
    }
}