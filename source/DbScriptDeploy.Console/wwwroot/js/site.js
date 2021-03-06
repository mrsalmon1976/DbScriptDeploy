﻿Vue.mixin({
    methods: {
        formatDate: function (dt, format) {
            var f = format;
            if (f === undefined) { f = 'YYYY-MM-DD HH:mm:ss' };
            return dayjs(dt).format(f);
        },
    },
});

var masterMenuApp = new Vue({
    el: '#master_menu',
    data: {
        isProcessing: false,
        isLoadingProjects: true,
        projects: []
    },
    mounted: function () {
        this.loadProjects();
    },
    methods: {
        addProject: function () {
            var that = this;
            swal({
                title: "Add Project",
                text: "Enter the name of your project",
                type: "input",
                showCancelButton: true,
                closeOnConfirm: false,
                showLoaderOnConfirm: true,
                animation: "slide-from-top",
                inputPlaceholder: "Project name"
            }, function (inputValue) {
                if (inputValue === false) return false;
                if (inputValue === "") {
                    swal.showInputError("You need enter a project name"); return false;
                }
                var request = $.ajax({
                    url: '/api/project',
                    method: "POST",
                    contentType: 'application/json',
                    data: JSON.stringify({
                        name: inputValue
                    })
                });
                request.done(function (response) {
                    that.loadProjects();
                    swal("Success", "Your new project has been created.", "success");
                });
                request.fail(function (xhr, textStatus) {
                    swal("Error", 'An error occurred adding the project', "error");
                });
            });
        },
        loadProjects: function () {
            var that = this;
            that.isLoadingProjects = true;
            var request = $.ajax({
                url: '/api/project/user',
                method: "GET"
            });
            request.done(function (response) {
                that.projects = response;
            });
            request.fail(function (xhr, textStatus, errorThrown) {
                swal("Error", 'An error occurred loading projects', "error");
            });
            request.always(function (xhr, textStatus, errorThrown) {
                that.isLoadingProjects = false;
            });
        },
    }
});