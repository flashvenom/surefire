Use this cheat sheet when adding Syncfusion `SfDropDownList` or Fluent Combobox components in the Blazor app.

## Imports
```razor
@using Syncfusion.Blazor
@using Syncfusion.Blazor.DropDowns
```
Add page-specific namespaces only when needed.

## Minimal Syncfusion dropdown
```razor
<SfDropDownList TItem="string" TValue="string"
                DataSource="@Names"
                @bind-Value="SelectedName"
                Placeholder="Select">
</SfDropDownList>
```
- `DataSource` must be non-null (`Array.Empty<string>()` if you have no data yet)
- `Placeholder` keeps Fluent styling aligned

## Complex objects
```razor
<SfDropDownList TItem="Carrier" TValue="int?"
                DataSource="@Carriers"
                @bind-Value="SelectedCarrierId"
                AllowFiltering="true"
                @bind-Value:after="OnCarrierChanged">
    <DropDownListFieldSettings Text="CarrierName" Value="CarrierId" />
    <DropDownListEvents TValue="int?" TItem="Carrier"
                        ValueChange="OnCarrierChanged" />
</SfDropDownList>
```
- Always supply `DropDownListFieldSettings` when `TItem` is not a simple type
- When using `AllowFiltering`, provide either `FilterType` or a `Filter` callback, not both

## Templates
```razor
<DropDownListTemplates TItem="Carrier">
    <HeaderTemplate>
        <div class="carrier-header">
            <span>Carrier</span>
            <span>Type</span>
        </div>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="carrier-item">
            <span>@context.CarrierName</span>
            <span class="badge">@context.Type</span>
        </div>
    </ItemTemplate>
</DropDownListTemplates>
```
Remember: `@context` is typed as `TItem`.

## Fluent Combobox quick ref
```razor
<FluentCombobox @bind-Value="SelectedProduct"
                Items="@ProductOptions"
                ValueField="Value"
                TextField="Label"
                OnChange="OnProductChanged" />
```
Use Fluent Combobox for shorter lists when you want native Fluent styling.

## Validation
- Wrap dropdowns inside `<EditForm>` and use `ValidationMessage For="() => Model.Property"`
- For Syncfusion components embedded in forms, set `ValueExpression="() => Model.Property"`

## Anti-patterns
- Inventing attributes (`TItemType`, `PlaceholderText`, `FieldSettings`) - they do not exist
- Putting `ValueChange` on the root tag instead of in `<DropDownListEvents>`
- Self-closing `<SfDropDownList />` tags (rare, but avoid to keep markup consistent)

## Examples in code
- `Domain/Policies/Pages/Edit.razor` - carrier/wholesaler dropdowns with filtering
- `Domain/Leads/Components/Create.razor` - stage/client/product dropdowns

Copy from these examples to keep generated code consistent.
