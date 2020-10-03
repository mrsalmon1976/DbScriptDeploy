var projectEnvironmentApp = new Vue({
    el: '#project-page',
    data: {
        projectId: null,
        environments: [],
        isAddButtonVisible: true
    },
    mounted: function () {

        var that = this;

        this.projectId = $('#hid-project-id').val();

        //alert(this.projectId);
        this.loadEnvironments();

        // track when tabs change
        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            that.onTabChange(e);
        });
    },
    methods: {
        loadEnvironments: function () {
            var that = this;
            //that.isLoadingProjects = true;
            //var request = $.ajax({
            //    url: '/api/projects/environments/' + this.projectId,
            //    method: "GET"
            //});
            //request.done(function (response) {
            //    that.environments = response;
            //});
            //request.fail(function (xhr, textStatus) {
            //    swal("Error", error, "error");
            //});
            //request.always(function (xhr, textStatus, errorThrown) {
            //    //that.isLoadingProjects = false;
            //});
        },
        onAddButtonClick: function () {
            if ($('#tab-scripts').hasClass('active')) {
                alert('scripts active');
                var url = '/project/' + this.projectId + '/editor';
                alert(url);
                window.location.href = url;
            }
            else if ($('#tab-environments').hasClass('active')) {
                alert('environments active');
            }
        },
        onTabChange: function (e) {
            var target = $(e.target).attr("href"); // activated tab
            if (target == '#tab_environments') {
                this.isAddButtonVisible = true;
            }
            else if (target == '#tab_scripts') {
                this.isAddButtonVisible = true;
            }
            else {
                this.isAddButtonVisible = false;
            }
        }
    }
});