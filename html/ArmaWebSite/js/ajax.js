
function LoadAddons(param) {
    $.ajax({
        type: "POST",
        url: param.url,
        data: { filter: param.filter, sorting: param.sorting }
    })
  .done(function (msg) {

            $(param.element).html(msg);

        });
}

