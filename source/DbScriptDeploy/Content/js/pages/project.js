$(document).ready(function () {
    $('#form_environment').validate({
        rules: {
            environmentName: {
                required: true,
            },
            environmentDbType: {
                required: true,
                number: true,
                min: 1
            }
        }, 
        messages: {
            environmentDbType: {
                min: "This field is required"
            },
        },
        onkeyup: function (element) { $(element).valid() },
        highlight: function (input) {
            //console.log(input);
            $(input).parents('.form-line').addClass('error');
        },
        unhighlight: function (input) {
            $(input).parents('.form-line').removeClass('error');
        },
        errorPlacement: function (error, element) {
            $(element).parents('.form-group').append(error);
        }
    });
});

var projectEnvironmentApp = new Vue({
    el: '#project-page',
    data: {
        projectId: null,
        databaseTypes: [{ id: '0', name: 'None' }],
        designations: [{ id: '0', name: 'None' }],
        environments: [],
        scripts: [],
        runnableScripts: [],
        isAddButtonVisible: true,
        isLoadingEnvironments: true,
        isLoadingScripts: true,

        // environment details
        isSavingEnvironment: false,
        environmentName: '',
        environmentDbType: '0',
        environmentHost: '',
        environmentPort: '',
        environmentDatabase: '',
        environmentUserName: '',
        environmentPassword: '',
        environmentDesignation: '0',

        // scripts information
        selectScriptToggle: false,
    },
    mounted: function () {

        var that = this;

        this.projectId = $('#hid-project-id').val();

        // load data
        this.loadEnvironments();
        this.loadDatabaseTypes();
        this.loadDesignations();
        this.loadScripts();

        // track when tabs change
        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            that.onTabChange(e);
        });
    },
    methods: {
        getEnvironmentExecutionState(environmentId, script) {
            for (var i = 0; i < script.executions.length; i++) {
                if (script.executions[i].environmentId == environmentId) {
                    return true;
                }
            }
            return false;
        },
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
            var that = this
            that.isLoadingEnvironments = true;
            var request = $.ajax({
                url: '/api/project/' + this.projectId + '/environments',
                method: "GET"
            });
            request.done(function (response) {
                that.environments = response;
            });
            request.fail(function (xhr, textStatus, errorThrown) {
                swal("Error", "An error occurred fetching environments: " + errorThrown, "error");
            });
            request.always(function (xhr, textStatus, errorThrown) {
                that.isLoadingEnvironments = false;
            });
        },
        loadScripts: function () {
            var that = this;
            that.isLoadingScripts = true;
            var request = $.ajax({
                url: '/api/project/' + this.projectId + '/scripts',
                method: "GET"
            });
            request.done(function (response) {
                that.scripts = response;
            });
            request.fail(function (xhr, textStatus, errorThrown) {
                swal("Error", "An error occurred fetching environments: " + errorThrown, "error");
            });
            request.always(function (xhr, textStatus, errorThrown) {
                that.isLoadingScripts = false;
            });
        },
        onAddButtonClick: function () {
            if ($('#tab-scripts').hasClass('active')) {
                var url = '/project/' + this.projectId + '/editor';
                window.location.href = url;
            }
            else if ($('#tab-environments').hasClass('active')) {
                $('#dlgEnvironment').modal('show');
            }
        },
        onOpenRunDialogButtonClick: function () {
            this.runnableScripts.splice(0);
            for (var i = 0; i < this.scripts.length; i++) {
                if (this.scripts[i].checked) {
                    this.runnableScripts.push(this.scripts[i]);
                }
            }
            if (this.runnableScripts.length > 0) {
                $('#dlgRunScripts').modal('show');
            }
            else {
                swal({
                    title: "No scripts selected",
                    text: "You need to select at least one script to run",
                    type: "error",
                    buttons: { cancel: 'OK' }
                })
            }
        },
        onSelectedScriptsChange: function (e) {
            for (var i = 0; i < this.scripts.length; i++) {
                this.scripts[i].checked = this.selectScriptToggle;
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
        },
        saveEnvironment: function () {
            var that = this;
            if ($("#form_environment").validate().valid()) {
                that.isSavingEnvironment = true;
                var request = $.ajax({
                    url: '/api/environment',
                    method: "POST",
                    data: {
                        name: this.environmentName,
                        projectId: this.projectId,
                        dbType: this.environmentDbType,
                        host: this.environmentHost,
                        port: this.environmentPort,
                        database: this.environmentDatabase,
                        userName: this.environmentUserName,
                        password: this.environmentPassword,
                        designationId: this.environmentDesignation
                    }
                });
                request.done(function (response) {
                    //alert('saved');
                    //that.environments = response;
                    that.loadEnvironments();
                });
                request.fail(function (xhr, textStatus, errorThrown) {
                    swal("Error", 'An error occurred saving the environment: ' + xhr.statusText, "error");
                });
                request.always(function (xhr, textStatus, errorThrown) {
                    that.isSavingEnvironment = false;
                });
            }
        },
    }
});