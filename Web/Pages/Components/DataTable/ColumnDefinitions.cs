using Newtonsoft.Json;
using System;

namespace Web.Pages.Components.DataTable
{
    public class Col : ColumnDefinition
    {
        /// <summary>
        /// Empty constructor for full customization.
        /// </summary>
        public Col() { }


        /// <summary>
        /// Sets the field value to the name of the property provided (this can be overridden by explicitly setting the Field value.
        /// Sets the title value to a localised string obtained via a Display attribute if one is present on the property, or just
        /// the name of the property if not. Adds a red asterisk if the property is marked as required.
        /// If a value is provided for filterType, a header filter will be automatically generated.
        /// onfigures validation rules based on attributes set in the entity's class.
        /// 
        /// EXAMPLES
        /// new Col(typeof(Building), "BuildingNameEN");
        /// new Col(typeof(Directorate), nameof(Directorate.AcronymFR));
        /// new Col(typeof(Branch), "BranchID") { Field = "ID" }; (explicitly sets the Field value)
        /// new Col(typeof(Building), "BuildingNameEN", FilterType.ValuesDropdown);        /// 
        /// </summary>
        /// 
        /// <param name="entity">The System.Type of the EF class you're using for the table.</param>
        /// <param name="property">The name of the entity's property used for this column.</param>
        /// <param name="filterType">Automatically configure a header filter for the selected type.</param>
        public Col(Type entity, string property, Editor editorType, Filter filterType)
        {
            // Tabulator uses Field to assign data to columns. 
            // (i.e. if the table data object has a property called StreetAddress, those values will go into the column with Field = "StreetAddress")
            Field = property;

            // Get the property's metadata.
            var propInfo = entity.GetProperty(property);

            // Tabulator uses Title as the displayed column header text.
            Title = propInfo.GetColumnHeader();

            // Configure validation rules based on attributes set in the entity class.
            Validator = propInfo.BuildValidatorParams();


            // Configure editor for specified editorType. Checks property's attributes for necessary input restrictions.
            switch(editorType)
            {
                case DataTable.Editor.BooleanDropdown:
                    Editor = "select";
                    EditorParams = new EditorParams() { Values = Utils.ValuesToDictionary(Utils.BoolDropdownValues()) };
                    Formatter = "lookup";
                    FormatterParams = Utils.ValuesToDictionary(Utils.BoolDropdownValues());
                    break;

                case DataTable.Editor.Text:
                    Editor = "input";
                    var editorParams = propInfo.BuildEditorParams();
                    if (editorParams != null)
                    {
                        EditorParams = editorParams;
                    }
                    break;
            }


            // Configure header filter for specified filterType.
            switch(filterType)
            {
                case Filter.BooleanDropdown:
                    HeaderFilter = "select";
                    HeaderFilterPlaceholder = DataTableResources.FilterColumn;
                    HeaderFilterParams = new HeaderFilterParams() { Values = Utils.BoolDropdownValues() };
                    break;

                case Filter.Text:
                    HeaderFilter = "input";
                    HeaderFilterPlaceholder = DataTableResources.FilterColumn;
                    break;

                case Filter.ValuesDropdown:
                    HeaderFilter = "select";
                    HeaderFilterPlaceholder = DataTableResources.FilterColumn;
                    HeaderFilterParams = new HeaderFilterParams() { Values = true, SortValuesList = "asc" };
                    break;
            }
        }        
    }
    
    public class NavCol : ColumnDefinition
    {
        /// <summary>
        /// Sets the field value to the name of the property provided (this can be overridden by explicitly setting the Field value.
        /// Sets the title value to a localised string obtained via a Display attribute if one is present on the property, or just
        /// the name of the property if not. Adds a red asterisk if the property is marked as required.
        /// Configures a dropdown for value selection and configures display text in the cells and the dropdown.
        /// </summary>
        /// 
        /// <param name="entity">The System.Type of the EF class you're using for the table.</param>
        /// <param name="property">The name of the entity's property used for this column.</param>
        /// <param name="displayValues">Values available for use in this column. Labels will be displayed in the selection dropdown and the cells.</param>
        /// <param name="editorValues">If this is not null, use these values for the dropdown.</param>
        public NavCol(Type entity, string property, Val[] displayValues, Val[] editorValues = null)
        {
            // Tabulator uses Field to assign data to columns. 
            // (i.e. if the table data object has a property called StreetAddress, those values will go into the column with Field = "StreetAddress")
            Field = property;

            // Get the property's metadata.
            var propInfo = entity.GetProperty(property);

            // Tabulator uses Title as the displayed column header text.
            Title = propInfo.GetColumnHeader();

            // Configure validation rules based on attributes set in the entity class.
            var validator = propInfo.BuildValidatorParams();
            if (validator.Length > 0)
            {
                Validator = validator;
            }            

            // Configure display text using lookup values.
            Formatter = "lookup";
            FormatterParams = Utils.ValuesToDictionary(displayValues);
            
            // Configure the editor use use the list of values provided. If editorValues is not explicitly provided, use displayValues.
            Editor = "select";
            EditorParams = new EditorParams() { Values = Utils.ValuesToDictionary(editorValues ?? displayValues) };


            // Configure header filter.
            HeaderFilter = "select";
            HeaderFilterPlaceholder = DataTableResources.FilterColumn;
            HeaderFilterParams = new HeaderFilterParams() { Values = displayValues, SortValuesList = "asc" };
            HeaderFilterFunc = "=";
        }    
    }

    public abstract class ColumnDefinition
    {
        [JsonProperty("accessor", NullValueHandling = NullValueHandling.Ignore)]
        public string Accessor { get; set; }

        [JsonProperty("accessorClipboard", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessorClipboard { get; set; }

        [JsonProperty("accessorClipboardParams", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessorClipboardParams { get; set; }

        [JsonProperty("accessorData", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessorData { get; set; }

        [JsonProperty("accessorDataParams", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessorDataParams { get; set; }

        [JsonProperty("accessorDownload", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessorDownload { get; set; }

        [JsonProperty("accessorDownloadParams", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessorDownloadParams { get; set; }

        [JsonProperty("accessorParams", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessorParams { get; set; }

        [JsonProperty("bottomCalc", NullValueHandling = NullValueHandling.Ignore)]
        public string BottomCalc { get; set; }

        [JsonProperty("bottomCalcFormatter", NullValueHandling = NullValueHandling.Ignore)]
        public string BottomCalcFormatter { get; set; }

        [JsonProperty("bottomCalcFormatterParams", NullValueHandling = NullValueHandling.Ignore)]
        public string BottomCalcFormatterParams { get; set; }

        [JsonProperty("bottomCalcParams", NullValueHandling = NullValueHandling.Ignore)]
        public string BottomCalcParams { get; set; }

        [JsonProperty("cellClick", NullValueHandling = NullValueHandling.Ignore)]
        public string CellClick { get; set; }

        [JsonProperty("cellContext", NullValueHandling = NullValueHandling.Ignore)]
        public string CellContext { get; set; }

        [JsonProperty("cellDblClick", NullValueHandling = NullValueHandling.Ignore)]
        public string CellDblClick { get; set; }

        [JsonProperty("cellDblTap", NullValueHandling = NullValueHandling.Ignore)]
        public string CellDblTap { get; set; }

        [JsonProperty("cellEditCancelled", NullValueHandling = NullValueHandling.Ignore)]
        public string CellEditCancelled { get; set; }

        [JsonProperty("cellEdited", NullValueHandling = NullValueHandling.Ignore)]
        public string CellEdited { get; set; }

        [JsonProperty("cellEditing", NullValueHandling = NullValueHandling.Ignore)]
        public string CellEditing { get; set; }

        [JsonProperty("cellMouseEnter", NullValueHandling = NullValueHandling.Ignore)]
        public string CellMouseEnter { get; set; }

        [JsonProperty("cellMouseLeave", NullValueHandling = NullValueHandling.Ignore)]
        public string CellMouseLeave { get; set; }

        [JsonProperty("cellMouseMove", NullValueHandling = NullValueHandling.Ignore)]
        public string CellMouseMove { get; set; }

        [JsonProperty("cellMouseOut", NullValueHandling = NullValueHandling.Ignore)]
        public string CellMouseOut { get; set; }

        [JsonProperty("cellMouseOver", NullValueHandling = NullValueHandling.Ignore)]
        public string CellMouseOver { get; set; }

        [JsonProperty("cellTap", NullValueHandling = NullValueHandling.Ignore)]
        public string CellTap { get; set; }

        [JsonProperty("cellTapHold", NullValueHandling = NullValueHandling.Ignore)]
        public string CellTapHold { get; set; }

        [JsonProperty("clicktMenu", NullValueHandling = NullValueHandling.Ignore)]
        public string ClicktMenu { get; set; }

        [JsonProperty("clipboard", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Clipboard { get; set; }

        [JsonProperty("contextMenu", NullValueHandling = NullValueHandling.Ignore)]
        public string ContextMenu { get; set; }

        [JsonProperty("cssClass", NullValueHandling = NullValueHandling.Ignore)]
        public string CssClass { get; set; }

        [JsonProperty("download", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Download { get; set; }

        [JsonProperty("downloadTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string DownloadTitle { get; set; }

        [JsonProperty("editable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Editable { get; set; }

        [JsonProperty("editableTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string EditableTitle { get; set; }

        [JsonProperty("editor", NullValueHandling = NullValueHandling.Ignore)]
        public string Editor { get; set; }

        [JsonProperty("editorParams", NullValueHandling = NullValueHandling.Ignore)]
        public EditorParams EditorParams { get; set; }

        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }

        [JsonProperty("formatter", NullValueHandling = NullValueHandling.Ignore)]
        public string Formatter { get; set; }

        [JsonProperty("formatterClipboard", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatterClipboard { get; set; }

        [JsonProperty("formatterClipboardParams", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatterClipboardParams { get; set; }

        [JsonProperty("formatterHtmlOutput", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatterHtmlOutput { get; set; }

        [JsonProperty("formatterHtmlOutputParams", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatterHtmlOutputParams { get; set; }

        [JsonProperty("formatterParams", NullValueHandling = NullValueHandling.Ignore)]
        public object FormatterParams { get; set; }

        [JsonProperty("formatterPrint", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatterPrint { get; set; }

        [JsonProperty("formatterPrintParams", NullValueHandling = NullValueHandling.Ignore)]
        public string FormatterPrintParams { get; set; }

        [JsonProperty("frozen", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Frozen { get; set; }

        [JsonProperty("headerClick", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderClick { get; set; }

        [JsonProperty("headerContext", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderContext { get; set; }

        [JsonProperty("headerContextMenu", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderContextMenu { get; set; }

        [JsonProperty("headerDblClick", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderDblClick { get; set; }

        [JsonProperty("headerDblTap", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderDblTap { get; set; }

        [JsonProperty("headerFilter", NullValueHandling = NullValueHandling.Ignore)]
        public object HeaderFilter { get; set; }

        [JsonProperty("headerFilterEmptyCheck", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderFilterEmptyCheck { get; set; }

        [JsonProperty("headerFilterFunc", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderFilterFunc { get; set; }

        [JsonProperty("headerFilterFuncParams", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderFilterFuncParams { get; set; }

        [JsonProperty("headerFilterLiveFilter", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderFilterLiveFilter { get; set; }

        [JsonProperty("headerFilterParams", NullValueHandling = NullValueHandling.Ignore)]
        public HeaderFilterParams HeaderFilterParams { get; set; }

        [JsonProperty("headerFilterPlaceholder", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderFilterPlaceholder { get; set; }

        [JsonProperty("headerHozAlign", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderHozAlign { get; set; }

        [JsonProperty("headerMenu", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderMenu { get; set; }

        [JsonProperty("headerSort", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HeaderSort { get; set; }

        [JsonProperty("headerSortStartingDir", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderSortStartingDir { get; set; }

        [JsonProperty("headerSortTristate", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderSortTristate { get; set; }

        [JsonProperty("headerTap", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderTap { get; set; }

        [JsonProperty("headerTapHold", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderTapHold { get; set; }

        [JsonProperty("headerTooltip", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderTooltip { get; set; }

        [JsonProperty("headerVertical", NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderVertical { get; set; }

        [JsonProperty("hozAlign", NullValueHandling = NullValueHandling.Ignore)]
        public string HozAlign { get; set; }

        [JsonProperty("htmlOutput", NullValueHandling = NullValueHandling.Ignore)]
        public string HtmlOutput { get; set; }

        [JsonProperty("minWidth", NullValueHandling = NullValueHandling.Ignore)]
        public string MinWidth { get; set; }

        [JsonProperty("mutator", NullValueHandling = NullValueHandling.Ignore)]
        public string Mutator { get; set; }

        [JsonProperty("mutatorClipboard", NullValueHandling = NullValueHandling.Ignore)]
        public string MutatorClipboard { get; set; }

        [JsonProperty("mutatorClipboardParams", NullValueHandling = NullValueHandling.Ignore)]
        public string MutatorClipboardParams { get; set; }

        [JsonProperty("mutatorData", NullValueHandling = NullValueHandling.Ignore)]
        public string MutatorData { get; set; }

        [JsonProperty("mutatorDataParams", NullValueHandling = NullValueHandling.Ignore)]
        public string MutatorDataParams { get; set; }

        [JsonProperty("mutatorEdit", NullValueHandling = NullValueHandling.Ignore)]
        public string MutatorEdit { get; set; }

        [JsonProperty("mutatorEditParams", NullValueHandling = NullValueHandling.Ignore)]
        public string MutatorEditParams { get; set; }

        [JsonProperty("mutatorParams", NullValueHandling = NullValueHandling.Ignore)]
        public string MutatorParams { get; set; }

        [JsonProperty("print", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Print { get; set; }

        [JsonProperty("resizable", NullValueHandling = NullValueHandling.Ignore)]
        public string Resizable { get; set; }

        [JsonProperty("responsive", NullValueHandling = NullValueHandling.Ignore)]
        public long? Responsive { get; set; }

        [JsonProperty("rowHandle", NullValueHandling = NullValueHandling.Ignore)]
        public string RowHandle { get; set; }

        [JsonProperty("sorter", NullValueHandling = NullValueHandling.Ignore)]
        public string Sorter { get; set; }

        [JsonProperty("sorterParams", NullValueHandling = NullValueHandling.Ignore)]
        public string SorterParams { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("titleFormatter", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleFormatter { get; set; }

        [JsonProperty("titleFormatterParams", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleFormatterParams { get; set; }

        [JsonProperty("tooltip", NullValueHandling = NullValueHandling.Ignore)]
        public string Tooltip { get; set; }

        [JsonProperty("topCalc", NullValueHandling = NullValueHandling.Ignore)]
        public string TopCalc { get; set; }

        [JsonProperty("topCalcFormatter", NullValueHandling = NullValueHandling.Ignore)]
        public string TopCalcFormatter { get; set; }

        [JsonProperty("topCalcFormatterParams", NullValueHandling = NullValueHandling.Ignore)]
        public string TopCalcFormatterParams { get; set; }

        [JsonProperty("topCalcParams", NullValueHandling = NullValueHandling.Ignore)]
        public string TopCalcParams { get; set; }

        [JsonProperty("validator", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Validator { get; set; }

        [JsonProperty("vertAlign", NullValueHandling = NullValueHandling.Ignore)]
        public string VertAlign { get; set; }

        [JsonProperty("visible", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Visible { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public string Width { get; set; }

        [JsonProperty("widthGrow", NullValueHandling = NullValueHandling.Ignore)]
        public string WidthGrow { get; set; }

        [JsonProperty("widthShrink", NullValueHandling = NullValueHandling.Ignore)]
        public string WidthShrink { get; set; }
    }

    public class EditorParams
    {
        [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultValue { get; set; }

        [JsonProperty("elementAttributes", NullValueHandling = NullValueHandling.Ignore)]
        public ElementAttributes ElementAttributes { get; set; }

        [JsonProperty("min", NullValueHandling = NullValueHandling.Ignore)]
        public string Min { get; set; }

        [JsonProperty("mask", NullValueHandling = NullValueHandling.Ignore)]
        public string Mask { get; set; }

        [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
        public string Max { get; set; }

        [JsonProperty("multiselect", NullValueHandling = NullValueHandling.Ignore)]
        public string Multiselect { get; set; }

        [JsonProperty("search", NullValueHandling = NullValueHandling.Ignore)]
        public string Search { get; set; }

        [JsonProperty("sortValuesList", NullValueHandling = NullValueHandling.Ignore)]
        public string SortValuesList { get; set; }

        [JsonProperty("step", NullValueHandling = NullValueHandling.Ignore)]
        public string Step { get; set; }

        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public object Values { get; set; }

        [JsonProperty("verticalNavigation", NullValueHandling = NullValueHandling.Ignore)]
        public string VerticalNavigation { get; set; }
    }

    public class ElementAttributes
    {
        [JsonProperty("maxlength", NullValueHandling = NullValueHandling.Ignore)]
        public string MaxLength { get; set; }
    }

    public class FormatterParams
    {
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public object Values { get; set; }
    }

    public class HeaderFilterParams
    {
        [JsonProperty("initial", NullValueHandling = NullValueHandling.Ignore)]
        public string Initial { get; set; }

        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public object Values { get; set; }

        [JsonProperty("sortValuesList", NullValueHandling = NullValueHandling.Ignore)]
        public string SortValuesList { get; set; }
    }

    public class Val
    {
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }
}