' ============================================================================
'  Models.vb
'  One class for each kind of record. Each class matches a table in the
'  Access database (see DataStore.vb for the table definitions).
' ============================================================================

''' <summary>One drug (one batch) in the pharmacy.  Table: Drugs</summary>
Public Class Drug
    Public Property ID As Integer
    Public Property DrugName As String
    Public Property Category As String
    Public Property Quantity As Integer
    Public Property UnitPrice As Decimal
    Public Property BatchNo As String
    Public Property ExpiryDate As Date
    Public Property SupplierName As String
    Public Property SupplierContact As String
    Public Property ReorderLevel As Integer
End Class

''' <summary>A company that supplies drugs.  Table: Suppliers</summary>
Public Class Supplier
    Public Property ID As Integer
    Public Property SupplierName As String
    Public Property ContactPerson As String
    Public Property Phone As String
    Public Property Email As String
    Public Property Address As String
    Public Property SuppliedDrugs As String
End Class

''' <summary>One line in the history log.  Table: History</summary>
Public Class HistoryEntry
    Public Property LogDate As Date
    Public Property DrugName As String
    Public Property ActionType As String
    Public Property QtyChange As Integer
    Public Property Party As String          ' supplier or customer
    Public Property BatchNo As String
    Public Property UserName As String
End Class

''' <summary>One receipt (one customer's purchase).  Table: Sales</summary>
Public Class Sale
    Public Property SaleID As Integer
    Public Property SaleDate As Date
    Public Property Customer As String
    Public Property SoldBy As String
    Public Property Total As Decimal
    Public Property AmountPaid As Decimal
    Public Property Discount As Decimal          ' money taken off the subtotal
    Public Property PaymentMethod As String       ' Cash, Mobile Money or Card
    Public Property Voided As Boolean             ' True when the sale was cancelled / refunded
    Public Property VoidReason As String

    ''' <summary>Total before the discount.</summary>
    Public ReadOnly Property Subtotal As Decimal
        Get
            Return Total + Discount
        End Get
    End Property
End Class

''' <summary>One drug line on a receipt.  Table: SaleItems. Also used for the counter's cart.</summary>
Public Class SaleItem
    Public Property SaleID As Integer
    Public Property SaleDate As Date
    Public Property DrugID As Integer
    Public Property DrugName As String
    Public Property BatchNo As String
    Public Property Quantity As Integer
    Public Property UnitPrice As Decimal

    ''' <summary>Quantity x price.</summary>
    Public ReadOnly Property Total As Decimal
        Get
            Return Quantity * UnitPrice
        End Get
    End Property
End Class

''' <summary>Stock thrown away (for example, expired).  Table: Losses</summary>
Public Class LossRecord
    Public Property LossDate As Date
    Public Property DrugName As String
    Public Property BatchNo As String
    Public Property Quantity As Integer
    Public Property Value As Decimal
    Public Property Reason As String
End Class

''' <summary>A person who can sign in.  Table: Users</summary>
Public Class AppUser
    Public Property ID As Integer
    Public Property Username As String
    Public Property FullName As String
    Public Property Role As String           ' "Admin" or "Staff"
    Public Property IsActive As Boolean
End Class
