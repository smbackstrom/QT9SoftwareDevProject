<%@ Page Title="Movies" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Movies.WebForms._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <section class="row" aria-labelledby="aspnetTitle">
            <h1 id="aspnetTitle">Movies</h1>
           
        </section>

        <div class="row"> 
                <p>
                    <asp:Label 
                        ID="ErrorLabel" 
                        runat="server"
                        CssClass="error-message">
                    </asp:Label>

                   <div class="movie-toolbar, d-flex justify-content-between align-items-center mb-3">

                        <asp:LinkButton
                            ID="AddMovieButton"
                            runat="server"
                            CssClass="btn btn-success"
                            OnClick="AddMovieButton_Click">

                            <i class="bi bi-plus-circle"></i>
                            Movie
                        </asp:LinkButton>

                        <asp:HiddenField ID="SelectedMovieId" runat="server" />
                       

                       <div class="d-flex align-items-center">
                            <span class="toggle-text"> Show Inactive </span>
                            <label class="switch">
                                <input
                                    id="chkShowInactive"
                                    runat="server"
                                    type="checkbox"
                                    onchange="this.form.submit();"
                                    onserverchange="chkShowInactive_ServerChange" />

                                <span class="slider"></span>
                            </label>
                        </div>
                </div>

                    <asp:Panel ID="MovieEditorPanel" runat="server" CssClass="card mb-3" Visible="false">

                        <div class="card-header">
                            <asp:Label
                                ID="EditorTitleLabel"
                                runat="server"
                                Text="Add Movie" />
                        </div>

                        <div class="card-body">

                            <asp:HiddenField
                                ID="HiddenField1"
                                runat="server" />

                        <div class="form-group mb-3">
                            <label>Movie Title</label>
                            <asp:TextBox
                                ID="MovieTitleTextBox"
                                runat="server"
                                CssClass="form-control" />
                        </div>

                        <div class="form-group mb-3">
                            <label>Rating</label>
                            <asp:DropDownList
                                ID="MovieRatingDropDown"
                                runat="server"
                                CssClass="form-select">
                                <asp:ListItem>G</asp:ListItem>
                                <asp:ListItem>PG</asp:ListItem>
                                <asp:ListItem>PG-13</asp:ListItem>
                                <asp:ListItem>R</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group mb-3">
                            <label>Release Year</label>
                            <asp:TextBox
                                ID="ReleaseYearTextBox"
                                runat="server"
                                CssClass="form-control" />
                        </div>

                        <div class="form-check mb-3">
                             <label>Active</label>
                            <asp:CheckBox
                                ID="IsActiveCheckBox"
                                runat="server"
                                CssClass="form-check-input" />
                        </div>

                        <asp:Button
                            ID="SaveMovieButton"
                            runat="server"
                            Text="Add Movie"
                            CssClass="btn btn-primary"
                            onClick ="SaveButton_Click"/>

                        <asp:Button
                            ID="CancelEditButton"
                            runat="server"
                            Text="Cancel"
                            Visible="false"
                            CssClass="btn btn-secondary ms-2" 
                            onClick ="CancelMovieForm_Click"/>

                        </div>

                    </asp:Panel>

                    <asp:GridView 
                        ID="MovieGrid" 
                        runat="server" 
                        AutoGenerateColumns="False"                         
                        CssClass="movie-grid"
                        GridLines="Both"
                        EmptyDataText="No movies were returned."
                        OnRowCommand="MovieGrid_RowCommandAsync">

                        <RowStyle CssClass="movie-row" />
                        <AlternatingRowStyle CssClass="movie-row-alt" />

                        <Columns>
                            
                            <asp:BoundField DataField="MovieID" HeaderText="Movie ID" />

                            <asp:BoundField DataField="MovieTitle" HeaderText="Title" />

                            <asp:BoundField DataField="MovieRating" HeaderText="Rating" />

                            <asp:BoundField DataField="ReleaseYear" HeaderText="Release Year" />

                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <asp:LinkButton
                                        ID="EditButton"
                                        runat="server"
                                         Text="<i class='bi bi-pencil'></i>"
                                        ToolTip="Edit Movie"
                                        CommandName="EditMovie"
                                        CommandArgument='<%# Eval("MovieID") %>'
                                        CssClass="movie-action"/>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:LinkButton
                                        ID="DeleteButton"
                                        runat="server"
                                        Text="<i class='bi bi-trash2'></i>"
                                        ToolTip="Delete Movie"
                                        CommandName="DeleteMovie"
                                        CommandArgument='<%# Eval("MovieID") %>'
                                        OnClientClick="return confirm('Are you sure you want to delete this movie?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView> 
                </p>
            
        </div>
    </main>

</asp:Content>
