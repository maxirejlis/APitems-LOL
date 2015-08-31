(function (self, $, undefined) {
    self.Initialize = function () {
        $(".champion").bind("click", getData);
        $(".champion-all").bind("click", getData);
        $(".role-filter").bind("change", updateAvailableChampions);
        $(".rank-filter").bind("change", updateAvailableChampions);
        $('#myModal').on('shown.bs.modal', function () {
            $('#myInput').empty();
            $('#myInput').focus();
        })
        $("#charts").hide();
        $("#tables").hide();
        updateAvailableChampions();
    };
   
    var tabledata511 = $(".item-by-minute-511").find(".item-by-minute-data");
    var tabledata514 = $(".item-by-minute-514").find(".item-by-minute-data");

    function toggleScrollbar() {
        $(".page-scroll").css("cursor", "pointer");
        $(".page-scroll").attr("disabled", false);
        $(".page-scroll").attr("readonly", false);
    }

    function clearInfo() {
        $(".message").hide();
        $("#charts").hide();
        $("#tables").hide();

        waitingDialog.show();

        tabledata511.empty();
        tabledata514.empty();
    }

    function getData(e)
    {
        clearInfo();

        var nombre = $(this).data("nombre");
        var id = $(this).data("id");
        var role = $(".role-filter").val();
        var rank = $(".rank-filter").val();

        $.ajax({
            type: "Post",
            url: window.UrlGetChampionData,
            data: {"id" : id, "role":role, "rank":rank},
            success: function (result) {
                toggleScrollbar();
                
                populateChart(nombre, result.data);
                populateTables(result.data);
                $(".message").html("Potions and Biscuits excluded").show();
                waitingDialog.hide();
                window.location.href = $(".navbar-nav").find("a[href*=#charts]").attr("href");
                
            },
            error: function (er, d, message) {
                waitingDialog.hide();
                alert('Oops, an error ocurred :c');
            }
        });
    }

    function updateAvailableChampions(){
        var role = $(".role-filter").val();
        var rank = $(".rank-filter").val();
        $(".champion").each(function () { $(this).parent().parent().hide() });
        $.ajax({
            type: "Post",
            url: window.UrlGetAvailableChampions,
            data: {"role": role, "rank": rank },
            success: function (result) {
                $(result.data).each(function(){
                    $(".champion[data-id='" + this +"']").parent().parent().show();
                });
            }

        });
    }

    function populateChart(nombre, data) {
        $("#charts").show();
        var chart = new google.visualization.AreaChart(document.getElementById('ap-chart'));
        var options = {
            title: 'Average AP/MR - ' + nombre + ' By Patch',
            hAxis: { title: 'Minute', titleTextStyle: { color: '#333' } },
            vAxis: { minValue: 0 }
        };
        chart.draw(google.visualization.arrayToDataTable(data.tableData), options);
    }

    function populateTables(data) {
        var template = $("#template-row-item-data");
        var templateItem = $("#template-item-data");
        $(data.itemsData.itemsP11).each(function (i) {

            var t = $(template.html()).clone(true);
            t.find(".table-minute").html(this.Minute);
            t.find(".table-totalGames").html(this.TotalGames);

            $(this.Items).each(function (i) {
                var templateItemclone = $(templateItem.html()).clone(true);
                templateItemclone.find(".item-image").attr("src", this.ImageUrl);
                templateItemclone.find(".item-image").attr("title", this.Name);
                templateItemclone.find(".item-pickrate").html(this.PickRate);

                $(t.find(".item")).append(templateItemclone);
            });
            tabledata511.append(t);
        });

        $(data.itemsData.itemsP14).each(function (i) {

            var t = $(template.html()).clone(true);
            t.find(".table-minute").html(this.Minute);
            t.find(".table-totalGames").html(this.TotalGames);

            $(this.Items).each(function (i) {
                var templateItemclone = $(templateItem.html()).clone(true);
                templateItemclone.find(".item-image").attr("src", this.ImageUrl);
                templateItemclone.find(".item-image").attr("title", this.Name);
                templateItemclone.find(".item-pickrate").html(this.PickRate);

                $(t.find(".item")).append(templateItemclone);
            });
            tabledata514.append(t);
        });

        $("#tables").show();
    }

    function populateTimelines(data) {
            var container = document.getElementById('timeline');
            var chart = new google.visualization.Timeline(container);
            var dataTable = new google.visualization.DataTable();

            dataTable.addColumn({ type: 'string', id: 'President' });
            dataTable.addColumn({ type: 'date', id: 'Start' });
            dataTable.addColumn({ type: 'date', id: 'End' });
            dataTable.addRows([
              ['Washington', new Date(1789, 3, 30), new Date(1797, 2, 4)],
              ['Adams', new Date(1797, 2, 4), new Date(1801, 2, 4)],
              ['Jefferson', new Date(1801, 2, 4), new Date(1809, 2, 4)]]);

            chart.draw(dataTable);
        
    }

}(window.Index = window.Index || {}, jQuery));

jQuery(function () {
    Index.Initialize();
});