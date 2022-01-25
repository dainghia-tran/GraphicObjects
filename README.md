# Lập trình Windows - 19_3

## Giới thiệu

-   Họ và tên: Trần Đại Nghĩa
-   MSSV: 18120480
-   Đồ án: Project Paint (Graphic Objects)

## Hướng dẫn chạy project

> Khởi chạy file "ProjectPaint.exe" trong thư mục "Release"

_hoặc_

> Build solution và copy các file \*.dll (từ các project **Line, Rectangle, Ellipse**) vào thư mục Debug/Release của project "ProjectPaint"

## Các yêu cầu đã hoàn thành

1. Dynamically load all graphic objects that can be drawn from external DLL files
2. The user can choose which object to draw
3. The user can see the preview of the object they want to draw
4. The user can finish the drawing preview and their change becomes permanent with previously drawn objects
5. The list of drawn objects can be saved and loaded again for continuing later (JSON format)
6. Save and load all drawn objects as an image PNG format.

## Các yêu cầu nâng cao

1. Allow the user to change:
    - Color
    - Pen Width
    - Stroke type: dash, dot, dash dot dot
2. Adding image to the canvas
3. Zooming
4. Undo, Redo
5. Draw Line vertically/horizontally, Square (Rectangle), Circle (Ellipse) when pressing Shift
