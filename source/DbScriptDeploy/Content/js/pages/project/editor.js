var projectEditorApp = new Vue({
    el: '#editor-page',
    data: {
        projectId: null,
    },
    mounted: function () {

        var editorUp = ace.edit("editor_up");
        editorUp.setTheme("ace/theme/sqlserver");
        // set this to /ace/mode/sql if not SqlServer database
        editorUp.session.setMode("ace/mode/sqlserver");
        editorUp.setReadOnly(false); 
        document.getElementById('editor_up').style.fontSize = '14px';

        var editorDown = ace.edit("editor_down");
        editorDown.setTheme("ace/theme/sqlserver");
        // set this to /ace/mode/sql if not SqlServer database
        editorDown.session.setMode("ace/mode/sqlserver");
        editorDown.setReadOnly(false); 
        document.getElementById('editor_down').style.fontSize = '14px';
    },
    methods: {
    }
});