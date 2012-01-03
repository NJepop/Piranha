/**
* Common JQuery code for the manager area.
*/

// Keep track of the currently active tooltip
var active_tooltip = "";

$(document).ready(function () {
    //
    // Submit the form
    //
    $(".submit").click(function () {
        $("form").submit();
    });

    $(".info").click(function () {
        $(".help").slideToggle("medium");
        return false;
    });

    //
    // Toggle optional content
    //
    $(".expandable h2").click(function () {
        var h2 = $(this);
        h2.siblings(".optional").slideToggle("fast", function () {
            h2.toggleClass("expanded");
        });
    });

    //
    // Toggle tooltips
    //
    $(".toolbar li").hover(function () {
        var tooltip = $(this).children(".tooltip:first");
        var item = $(this);

        tooltip.css({ left: -((tooltip.outerWidth() - item.width()) / 2) });
        active_tooltip = item.attr("id");

        // We need to do a classic delay so we don't flicker 
        // when moving the mouse across the toolbar.
        setTimeout(function () {
            if (active_tooltip == item.attr("id"))
                item.children(".tooltip").fadeIn();
        }, 200);
    }, function () {
        if (active_tooltip == $(this).attr("id"))
            active_tooltip = "";
        $(this).children(".tooltip").fadeOut();
    });

    //
    // Tabs
    //
    $(".tabs a").click(function () {
        // First hide all tab data and remove the selected class
        $.each($(this).parent().siblings("li"), function (i, e) {
            $("a[name=" + $(e).children("a").attr("href").substring(1) + "]").addClass("hidden");
            $(this).children("a").removeClass("selected");
        });
        // Show current tab and set selected class
        $("a[name=" + $(this).attr("href").substring(1) + "]").removeClass("hidden");
        $(this).addClass("selected");
        return false;
    });

    //
    // Form validation errors
    //
    $(".field-validation-error").click(function () {
        $(this).fadeOut();
    });

    //
    // File uploads
    //
    $(".file").click(function () {
        $("#" + $(this).attr("data-id")).click();
        return false;
    });

    $("input[type=file]").change(function () {
        $("#" + $(this).attr("data-id")).val($(this).val());
    });
});

/**
 * JQuery code for the floatbox control.
 */

//
// Floatbox object definition.
//
function floatBoxDef() {
    //
    // Shows the floatbox with the given id.
    //
    this.show = function (id, width, height) {
        var outer = $("#" + id);
        var inner = outer.children(".box:first");

        // Show the box
        outer.fadeIn("medium");

        // Set dimensions, position and attach event handlers
        this.position(inner);
        outer.children(".bg:first").click(function () { floatBox.close(id) });

        return false;
    };

    //
    // Closes the box with the given id and unbind it's events
    //
    this.close = function (id) {
        $("#" + id).fadeOut("medium");
        $(this).unbind();
    };

    //
    // Positions the given box/boxes on the y-axis
    //
    this.position = function (box) {
        box.css({
            marginTop: Math.max(20, ($(window).height() - box.height() - 20) / 2),
            marginLeft: Math.max(20, ($(window).width() - box.width()) / 2)
        });
    }
}
var floatBox = new floatBoxDef();

//
// Window resize event
//
$(window).resize(function () {
    floatBox.position($(".floatbox .box"));
});

/**
 * JQuery code for the page views.
 */

$(document).ready(function () {
    // Show the first page region. TODO. WHAT IF THERE ARE NO PAGE REGIONS
    $("#pageregions .input:first").show();
    var firstid = $("#pageregions .input:first").attr("id");
    $("#" + firstid).addClass("active");
    $(".edit td").removeClass("active");
    $(".edit #" + firstid).addClass("active");

    // 
    // Event handler for page regions.
    //
    $(".pageregion").click(function () {
        var id = $(this).attr("id").substring(4);

        hideEditors();
        $("#pageregions #" + id).show();
        $(this).removeClass("blue").addClass("orange");
        $(".edit td").removeClass("active");
        $(".edit #" + id).addClass("active");

        return false;
    });

    //
    // Event hander for attachments.
    $("#btn_attachments").click(function () {
        hideEditors();
        $(this).removeClass("blue").addClass("orange");
        return false;
    });

    //
    // Hides all editors on the page.
    function hideEditors() {
        $("#pageregions .input, #globalregions .input").hide();
        $("#regionbuttons button").removeClass("orange").addClass("blue");
    }
});

/**
 * JQuery code for the template views.
 */

$(document).ready(function () {
    bindEvents();

    //
    // Process the form data some before sending it back to the server
    //
    $("form").submit(function () {
        // Build page regions
        $.each($("#pageregions").children(), function (index, val) {
            $("#region_data").append(
                '<input id="Template_PageRegions_' + index +
                '_" name="Template.PageRegions[' + index +
                ']" type="hidden" value="' + $(val).children("span:first").text() + '" />');
        });
        // Build Properties
        $.each($("#properties").children(), function (index, val) {
            $("#region_data").append(
                '<input id="Template_Properties_' + index +
                '_" name="Template.Properties[' + index +
                ']" type="hidden" value="' + $(val).children("span:first").text() + '" />');
        });
    });
});

//
// Binds the events associated with the region lists. This method is executed
// every time an item is added or removed as this updates the DOM.
//
function bindEvents() {
    $("#pr_add").unbind();
    $("#gr_add").unbind();
    $("#po_add").unbind();
    $(".remove-region").unbind();

    $("#pr_add").click(function () {
        var name = $("#pr_name").val();

        if (name != null && name != "") {
            $("#pageregions").append(
                '<li><span>' + name + '</span><button class="btn delete right remove-region"></button></li>');
            bindEvents();
        } else alert("Du måste ange ett namn för regionen.");
        return false;
    });

    $("#po_add").click(function () {
        var name = $("#po_name").val();

        if (name != null && name != "") {
            $("#properties").append(
                '<li><span>' + name + '</span><button class="btn delete right remove-region"></button></li>');
            bindEvents();
        } else alert("Du måste ange ett namn för egenskapen.");
        return false;
    });

    $(".remove-region").click(function () {
        $(this).parent().remove();
    });
}