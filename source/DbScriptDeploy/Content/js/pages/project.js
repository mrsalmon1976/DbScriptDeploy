var projectEnvironmentApp = new Vue({
    el: '#project-page',
    data: {
        projectId: null,
        databaseTypes: [{ id: 0, name: 'None' }],
        designations: [{ id: 0, name: 'None' }],
        environments: [],
        isAddButtonVisible: true,
        selectedDatabaseType: '0',
        selectedDesignation: '0',
    },
    mounted: function () {

        var that = this;

        this.projectId = $('#hid-project-id').val();

        // load data
        this.loadEnvironments();
        this.loadDatabaseTypes();
        this.loadDesignations();

        // track when tabs change
        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            that.onTabChange(e);
        });
    },
    methods: {
        loadDatabaseTypes: function () {
            var that = this;
            //that.isLoadingProjects = true;
            var request = $.ajax({
                url: '/api/lookup/databasetypes',
                method: "GET"
            });
            request.done(function (response) {
                for (var i = 0; i < response.length; i++) {
                    that.databaseTypes.push(response[i]);
                } 
            });
            request.fail(function (xhr, textStatus, errorThrown) {
                swal("Error", "An error occurred fetching database types: " + errorThrown, "error");
            });
            request.always(function (xhr, textStatus, errorThrown) {
                //that.isLoadingProjects = false;
            });
        },
        loadDesignations: function () {
            var that = this;
            //that.isLoadingProjects = true;
            var request = $.ajax({
                url: '/api/lookup/designations',
                method: "GET"
            });
            request.done(function (response) {
                for (var i = 0; i < response.length; i++) {
                    that.designations.push(response[i]);
                }
                //$('select:not(.ms)').selectpicker('refresh');
            });
            request.fail(function (xhr, textStatus, errorThrown) {
                swal("Error", "An error occurred fetching designations: " + errorThrown, "error");
            });
            request.always(function (xhr, textStatus, errorThrown) {
                //that.isLoadingProjects = false;
            });
        },
        loadEnvironments: function () {
            var that = this;
            //that.isLoadingProjects = true;
            //var request = $.ajax({
            //    url: '/api/project/' + this.projectId + '/environments',
            //    method: "GET"
            //});
            //request.done(function (response) {
            //    that.environments = response;
            //});
            //request.fail(function (xhr, textStatus, errorThrown) {
            //    swal("Error", errorThrown, "error");
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
                $('#dlgEnvironment').modal('show');
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