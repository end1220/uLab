﻿
using UnityEngine;


namespace Lite
{
	public class WallAvoiding : Steering
	{

		public WallAvoiding(KinematicComponent kinm) : 
			base(kinm)
		{

		}

		public override Vector3 Calculate()
		{
			Vector3 desiredVelocity = 
				Vector3.Normalize(m_kinematic.position - m_kinematic.targetPosition)
				* m_kinematic.maxSpeed;

			return (desiredVelocity - m_kinematic.velocity);
		}

	}
}
