// Write your Javascript code.

$(document).ready(function () {
    $("body").tooltip({ selector: '[data-toggle=tooltip]' });
});

$(document).on('click', '[data-toggle="lightbox"]', function (event) {
    event.preventDefault();
    $(this).ekkoLightbox();
});

$(document).ajaxStart($.blockUI)
    .ajaxStop($.unblockUI);

$(document).ready(function () {
    $("#product").autocomplete(
        {
            search: function (e, ui) {
                $(this).data("ui-autocomplete").menu.bindings = $();
            },
            source: function (request, response) {
                $.ajax(
                    {
                        url: "?handler=CartList",
                        dataType: "json",
                        data:
                        {
                            addressFilter: request.term
                        },
                        success: function (data) {
                            if (!data.length) {
                                var result = [
                                    {
                                        label: 'No matches found',
                                        value: response.term
                                    }
                                ];
                                response(result);
                            }
                            else {
                                response($.map(data, function (item) {
                                    return {
                                        label: item.full_address,
                                        value: item.full_address
                                    };
                                }));
                            }
                        }
                    });
            },
            minLength: 4,
            select: function (event, ui) {
                $("#product").val(ui.item.value);
                return false;
            }
        });
});


// this is the id of the form
//$("#cartAddItem").submit(function (e) {

//    var form = $(this);
//    var url = form.attr('action');

//    $.ajax({
//        type: "POST",
//        url: url,
//        data: form.serialize(), // serializes the form's elements.
//        success: function (data) {
//            $("#cartItemAdded").html('<i class="fa fa-plus fa-fw"></i>' + $("#productName").val() + " has been added to the cart!");
//            $("#cartItemAdded").show();
//            $("#cartItemCount").html(parseInt($("#cartItemCount").html()) + 1);
//        },
//        error: function (data) {
//            $("#cartItemAdded").html('<i class="fa fa-plus fa-fw"></i>' + $("#productName").val() + " failed!");
//            $("#cartItemAdded").removeClass('alert alert-success');
//            $("#cartItemAdded").addClass('alert alert-danger');
//            $("#cartItemAdded").show();
//        }
//    });

//    e.preventDefault(); // avoid to execute the actual submit of the form.
//});

$(document).ready(function () {
    $("form[name='add_cart']").submit(function (e) {

        var form = $(this);
        var url = form.attr('action');

        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(), // serializes the form's elements.
            success: function (data) {
                $("#cartItemAdded").html('<i class="fa fa-plus fa-fw"></i>' + data.productName + " has been added to the cart!");
                $("#cartItemAdded").show();
                $("#cartItemCount").html(data.cartCount);
            },
            error: function (data) {
                $("#cartItemAdded").html('<i class="fa fa-plus fa-fw"></i>' + $("#productName").val() + " failed!");
                $("#cartItemAdded").removeClass('alert alert-success');
                $("#cartItemAdded").addClass('alert alert-danger');
                $("#cartItemAdded").show();
            }
        });

        e.preventDefault(); // avoid to execute the actual submit of the form.
    });
});
$(document).ready(function () {
    var bLazy = new Blazy({
        offset: 1 //Load images at zero pixels from thew view port
    });
});