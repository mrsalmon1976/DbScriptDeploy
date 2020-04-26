﻿var masterMenuApp = new Vue({
    el: '#master_menu',
    data: {
        isProcessing: false,
    },
    methods: {
        addProject: function () {
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
                    var data = { name: inputValue };
                axios.post('/api/projects', data)
                    .then(function (response) {
                        swal("Success", "Your new project has been created.", "success");
                    })
                    .catch(function (error) {
                        swal("Error", error, "error");
                    })
                    .then(function () {
                        // always executed
                    });
            });
        }
    }
});