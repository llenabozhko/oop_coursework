   M S B A M L   `   `   `  ����I  Eoop_coursework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:oop_coursework.Viewsoop_coursework.Views  5   	   &     oop_coursework.Views.AdminWindow   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   4local"clr-namespace:oop_coursework.Views     5   	     � Title$/  )Панель адміністратора��5   	   $	��900��6'   $
��1200��64    � WindowStartupLocation$ CenterScreen=�5   	   .��5      � ���#F5F5F5.+�5   
   �� $��30q�6   
������� �� CornerRadius$ 8��5
      ���#E0E0E0$��1q�5       �� Padding$ 30q�5      ._�5      �  � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $��* �6$   .+�5      �� -   5         ��Name$ WelcomeText��5      $��0��5      $��24(�5      $��SemiBold"�5      $��0,0,0,30q�5      �� 5      $��1��6   
�������$ 8��5      $ 25q�5       $��0,0,0,30q�5!      ���#E0E0E0$��1q�5#      ._�5$      �  � ColumnDefinitions 5%      �� 5&      $
��Auto �6/   �� 5'      $
��Auto �6/   �� 5(      $��* �6/   .+�5+      �� -   5        $��0��5+   !   $)��#Додати користувача��5,   !   $��40��5.   !   $	��150��5/   !   $��0,0,25,0q�50   !   ���#2196F3
�������$��SemiBold"�53   !   �� 55      $��1��6%   $��Center=�56   #   .+�57      �� $"��Фільтр за роллю��6(   $��SemiBold"�58   &   $��0,0,0,8q�59   &   �� 5:      -   5         ��Name$ 
RoleFilter��5:   '   $	��200��5;   %   $��40��5<   %   $
��10,8q�5=   %   .@�5?   "   �� $#��Всі користувачі��6/   �� 5@   "   $��Студенти��6/   �� 5A   "   $��Викладачі��6/   �� 5B   "   $"��Адміністратори��6/   5C      5D      5E      5F      �� 5I      $��2��6   
�������$ 8��5K      $ 25q�5L      ���#E0E0E0$��1q�5N      ._�5O      �  5P      �� 5Q      $
��Auto �6,   �� 5R      $��* �6,   .+�5U      �� $5��/Управління користувачами��6$   $��SemiBold"�5V   "   $��18(�5W   "   $��0,0,0,20q�5X   "   &   System.Windows.Controls.DataGrid  5Z      -   5          Name$ 	UsersGrid��5[   !   $��1��5Z   #   $	��400��5\   !   	   AutoGenerateColumns$	 False��5]   !   
   
IsReadOnly$

 True��5^   !      HeadersVisibility$ Column=�5_   !      GridLinesVisibility$ 
Horizontal=�5`   !   $��1q�5a   !   ���#E0E0E0   RowHeaderWidth$ 0Q�5c   !      VerticalGridLinesBrush
 ����    HorizontalGridLinesBrush �#E0E0E0
������    RowBackground
 ����    AlternatingRowBackground �#F8F8F8$
��Auto=�5i   !   $
��True��5j   !      MinRowHeight$ 60Q�5k   !      	RowHeight$ 60Q�5l   !      EnableRowVirtualization$
 True��5m   !    %� IsVirtualizing$
 True��5n   !    %� VirtualizationMode$ 	Recycling=�5o   !    � 	Resources 5p      =  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value*  $System.Windows.Controls.DataGridCell �� 
TargetType �� Triggers P� Property P� Value��%;  '        '  �     �� 5q   "   "  6(   .�5r   &   ��  � ��
 ���� 6C   �� 5s   &    � ��$
 10,5q�6@   �� 5t   &    � ��$ SemiBold"�6C   5u   #   �� 5v   "   "  6(   .�5w   &   ��  � ��$
 10,5q�6@   �� 5x   &    � �� 5y   *   �� 5z   .   "  6>   .R�5{   2   �� # z��69   $��0q�5|   9   #��z��5}   9   ._�5~   6   �� 5   3   5�   /   5�   '    5�   &   P� 5�   *    �@ 
IsSelected$
 True��6H   . �5�   .   ��  � �� �#E3F2FD6K   �� 5�   .    � �� �#2196F36L   5�   +   5�   #   5�         Columns 5�      0  *System.Windows.Controls.DataGridTextColumn  5�   "   ,  &System.Windows.Controls.DataGridColumn    Header$  Повне ім'я��65   !   Width5  /System.Windows.Controls.DataGridLengthConverter$! 2* 5�   3   1  +System.Windows.Controls.DataGridBoundColumn"   Binding" 5�   3   �� *
FullName+5�   3     5�   "   $$  Ім'я користувача��65   $! * 5�   3   " 5�   3   �� *
Username+5�   3     5�   "   $  Роль��65   $! * 5�   3   " 5�   3   �� *Role+5�   3     5�   "   $#  Дата народження��65   $! * 5�   3   " 5�   3   �� *DateOfBirth+# �� StringFormat$# d��5�   3   4  .System.Windows.Controls.DataGridTemplateColumn  5�   "   $
! Auto 69   $   CellTemplate$ 5�   &   �� 5�   *   .R�5�   .   �� -   5        $��Видалити��5�   5   ���#DC3545
�������$��SemiBold"�5�   5   $
��15,5q�5�   5   $��5q�5�   5   
% � Tag% 5�   5   �� *Id+5�   5   5�   +   5�   #   5�      5�      5�      5�      5�      5�      5�      