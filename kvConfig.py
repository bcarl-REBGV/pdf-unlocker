drag_widget = '''
<DragWidget>:
    drag_rectangle: self.x, self.y, self.width, self.height
    drag_timeout: 10000
    drag_distance: 0

FloatLayout:
    DragLabel:
        size_hint: 0.25, 0.2
        text: 'Drag me'
'''