function CreatePieChart(capital, interest) {

	am5.array.each(am5.registry.rootElements, function (root) {
		if (root.dom.id == "chartdiv") {
			root.dispose();
		}
	});

	const root = am5.Root.new("chartdiv");

	root.setThemes([
		am5themes_Animated.new(root)
	]);

	var chart = root.container.children.push(am5percent.PieChart.new(root, {
		layout: root.verticalLayout
	}));


	var series = chart.series.push(am5percent.PieSeries.new(root, {
		valueField: "value",
		categoryField: "category"
	}));

	series.data.setAll([
		{ value: capital, category: "Pożyczony Kapitał" },
		{ value: interest, category: "Odsetki" },
	]);

	series.appear(1000, 100);
	return () => {
		root.dispose();
	};
}
