private CalendarDatePicker cdp;
private Calendar cd;
private Popup popup;
private CalendarMode mode = CalendarMode.Year;

public override void Initialise() {
	try {
		cdp = GetControlThrow<CalendarDatePicker>(nameof(cdp));
		cdp.TemplateApplied += CalendarDatePicker_TemplateApplied;
	}
	catch (Exception e) { Log.Error(e.Message); }
}

private void CalendarDatePicker_TemplateApplied(object? sender, TemplateAppliedEventArgs e) {
	if (cd == null) {
		try {
			cd = e.NameScope.Find<Calendar>("PART_Calendar");
			if (cd == null) {
				throw new Exception("PART_Calendar is null");
			}
			
			cd.DisplayMode = mode;
			cd.SelectedDatesChanged += Calendar_SelectedDatesChanged;
			cd.PropertyChanged += Calendar_PropertyChanged;
		}
		catch (Exception ex) { Log.Error(ex.Message); }
	}
	
	if (popup == null) {
		try {
			popup = e.NameScope.Find<Popup>("PART_Popup");
			if (popup == null) {
				throw new Exception("PART_Popup is null");
			}
		} 
		catch (Exception ex) { Log.Error(ex.Message); }
	}
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
			
			popup?.Close();
		}
	}
}