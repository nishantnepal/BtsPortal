$(function () {
    $(".datetimepicker").datetimepicker({});
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();
   
    $(".formShowLoadOnSubmit").submit(function () {
        waitingDialog.show();
    });

    $(".pagination").on("click", "a", function (e) {
        waitingDialog.show();
    });

    $('[data-toggle="popover"]').popover();

    $('.selectpicker').selectpicker({
        style: 'btn-info',
        size: 4
    });

    
});


