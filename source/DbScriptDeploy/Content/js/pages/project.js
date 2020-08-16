var globalData = {
    projectId: null,
    environments: []
}
globalData.projectId = $('#hid-project-id').val();

var projectEnvironmentApp = new Vue({
    el: '#project-page',
    data: globalData,
    mounted: function () {
        alert(this.projectId);
        this.loadEnvironments();
    },
    methods: {
        loadEnvironments: function () {
            var that = this;
            //that.isLoadingProjects = true;
            var request = $.ajax({
                url: '/api/projects/environments/' + this.projectId,
                method: "GET"
            });
            request.done(function (response) {
                that.environments = response;
            });
            request.fail(function (xhr, textStatus) {
                swal("Error", error, "error");
            });
            request.always(function (xhr, textStatus, errorThrown) {
                //that.isLoadingProjects = false;
            });
        },
    }
});