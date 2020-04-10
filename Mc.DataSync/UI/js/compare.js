var app = angular.module("app", []);
app.controller("ctrl", ["$scope", function ($scope) {

	//window.onerror = function (e) {

	//	//alert(e);
	//	return false;
	//};
	var vm = $scope;
	var dv;
	var service = window.external;
	function reload() {
		try {
            var res = null;
			if (service && service.GetDiff) {
				res = eval("(" + service.GetDiff() + ")");
			}
			else {
                res = {
                    oldValue: "未获取到旧的数据",
                    newValue: "未获取到新的数据",
                    fileName: "test.html",
                    fileExtension: ".html"
                };
            }
            initCompare(res);

		} catch (e) {
			alert(e);
		}

    }

    function initCompare(res) {

        var mode = "text/html";
        switch (res.fileExtension) {
            case ".js":
                mode = "text/javascript";
                break;
            case ".css":
                mode = "text/css";
                break;
            case ".html":
                mode = "text/html";
                break;
            case ".cs":
                mode = "text/x-csharp";
                break;
        }

        vm.fileName = res.fileName;

        dv = initUI(mode, res.oldValue, res.newValue);

    }

	reload();

	vm.goNext = function () {
		dv.edit.execCommand("goNextDiff");
	};
	vm.goPrev = function () {
		dv.edit.execCommand("goPrevDiff");
	};

    function initUI(mode,value1, value2) {
        if (value1 == null) return;
        var target = $("#view").html("")[0];

		return CodeMirror.MergeView(target, {
            value: value1,// 上次内容
            revertButtons: false,// 是否显示还原按钮
			origLeft: null,//origLeft, 中间对比合并内容
            orig: value2, //本次内容
            lineNumbers: true,// 显示行号
            mode: mode, //解析文本内容后缀 'text/html',
			theme: 'visual-studio-2012', //主题
			highlightDifferences: true, //是否高亮显示对比差异
			//connect: "align", //connect
			readOnly: true, //是否只读
			lineSeparator: '\r\n',//换行符号
			collapseIdentical: false //
		});
	}

	function toggleDifferences() {
		dv.setShowDifferences(highlight = !highlight);

    }

	function mergeViewHeight(mergeView) {
		function editorHeight(editor) {
			if (!editor) return 0;
			return editor.getScrollInfo().height;
		}
		return Math.max(editorHeight(mergeView.leftOriginal()),
			editorHeight(mergeView.editor()),
			editorHeight(mergeView.rightOriginal()));
	}

	function resize(mergeView) {
		var height = mergeViewHeight(mergeView);
		for (; ;) {
			if (mergeView.leftOriginal())
				mergeView.leftOriginal().setSize(null, height);
			mergeView.editor().setSize(null, height);
			if (mergeView.rightOriginal())
				mergeView.rightOriginal().setSize(null, height);

			var newHeight = mergeViewHeight(mergeView);
			if (newHeight >= height) break;
			else height = newHeight;
		}
		mergeView.wrap.style.height = height + "px";
	}

}]);
