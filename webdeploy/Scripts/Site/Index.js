(function (self, $, undefined) {
    self.Initialize = function () {
        $("#btnTest").click(test)
    };

    function test() {
        $.ajax({
            type: "Post",
            url: window.UrlGetData,
            success: function (data) {
                $('#result').html(data);
            },
            error: function (er, d, message) {

            }
        });
    }

}(window.Index = window.Index || {}, jQuery));

jQuery(function () {
    Index.Initialize();
});