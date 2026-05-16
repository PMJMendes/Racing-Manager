using Godot;
using System;

public partial class CameraRig : Node2D
{
	[Export]
	public Curve ZoomCurve;

	private Camera2D _camera;

	public void SweepCamera(float progress, float zoomPoint)
	{
		Position = new Vector2(35.0f + progress, 12.5f);
		var zoom = ZoomCurve.Sample(zoomPoint);
		_camera.Zoom = new Vector2(zoom, zoom);
	}

	public override void _Ready()
	{
		_camera = GetNode<Camera2D>("Camera2D");
		Position = new Vector2(35.0f, 12.5f);
		_camera.Zoom = new Vector2(10.0f, 10.0f);
	}
}
