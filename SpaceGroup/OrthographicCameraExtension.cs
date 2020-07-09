﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace SpaceGroup
{
    public static class OrthographicCameraExtension
    {

        public static void PositionCamera(this OrthographicCamera TheCamera, CrystalCell atomCell)
        {
            TheCamera.Position = new Point3D(0, atomCell.ZAxisL / 2, +50);
            TheCamera.LookDirection = new Vector3D(0, 0, -1);
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        public static void PositionCamera(this OrthographicCamera TheCamera)
        {
            double originalFieldOfView = 45.0;
            double scale = 5;
            double fov = Math.Atan(Math.Tan(originalFieldOfView / 2.0 / 180.0 * Math.PI) * scale) * 2.0;

            TheCamera.Position = new Point3D(0, 0, 50);
            TheCamera.LookDirection = new Vector3D(0, 0, -1);
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
            TheCamera.Width = fov / Math.PI * 180.0 * 0.1;
        }

        public static void RotateCamera(this OrthographicCamera TheCamera, double angleX, double angleY)
        {
            TheCamera.Rotate(new Vector3D(0, 1, 0), -angleX * 2);
            Vector3D rightDirection = TheCamera.LookDirection.Cross(TheCamera.UpDirection);
            TheCamera.Rotate(rightDirection, 2 * angleY);
        }

        public static void Rotate(this OrthographicCamera TheCamera, Vector3D axis, double angle)
        {
            Quaternion q = Math3D.Rotation(axis, angle);
            TheCamera.Position = q.Transform(TheCamera.Position);
            TheCamera.UpDirection = q.Transform(TheCamera.UpDirection);
            TheCamera.LookDirection = q.Transform(TheCamera.LookDirection);
        }

        public static void MoveRight(this OrthographicCamera TheCamera, double d)
        {
            double u = 0.05;
            Vector3D lookDirection = TheCamera.LookDirection;
            Point3D position = TheCamera.Position;

            lookDirection.Normalize();
            Vector3D rightDirection = TheCamera.LookDirection.Cross(TheCamera.UpDirection);
            position = position + u * rightDirection * Math.Sign(d);

            TheCamera.Position = position;
        }



        public static void MoveForward(double d)
        {
            //double u = 0.05;
            //OrthographicCamera camera = (OrthographicCamera)MainViewport.Camera;
            //Vector3D lookDirection = camera.LookDirection;
            //Point3D position = camera.Position;

            //lookDirection.Normalize();
            //position = position + u * lookDirection * d;

            //camera.Position = position;
        }

        public static void MoveUp(this OrthographicCamera TheCamera, double d)
        {
            double u = 0.05;
            Vector3D upDirection = TheCamera.UpDirection;
            Point3D position = TheCamera.Position;

            upDirection.Normalize();
            position = position + u * upDirection * Math.Sign(d);

            TheCamera.Position = position;
        }
    }
}
