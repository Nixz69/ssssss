private Calendar cd;
private CalendarMode mode = CalendarMode.Year;

public override void Initialise() {
	try {
		cd = GetControlThrow<Calendar>(nameof(cd));
		cd.DisplayMode = mode;
		cd.SelectedDatesChanged += Calendar_SelectedDatesChanged;
		cd.PropertyChanged += Calendar_PropertyChanged;
	}
	catch (Exception e) { Log.Error(e.Message); }
}

private void Calendar_SelectedDatesChanged(object? sender, SelectionChangedEventArgs e) {
	foreach (var item in e.AddedItems) 
		Log.Error(item.ToString());
	// or
	// Log.Error(cd.SelectedDate.ToString());
}

private void Calendar_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
	if (e.Property == Calendar.DisplayModeProperty) {
		if ((int)cd.DisplayMode < (int)mode) {
			cd.DisplayMode = mode;
			cd.SelectedDate = cd.DisplayDate;
		}
	}
}