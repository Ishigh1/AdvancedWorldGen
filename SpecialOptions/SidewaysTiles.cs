/*namespace AdvancedWorldGen.SpecialOptions;


public class SidewaysTiles : ModSystem
{
	public override void Load()
	{
		OnTileDrawing.DrawSingleTile += DrawSingleTile;
	}

	private void DrawSingleTile(OnTileDrawing.orig_DrawSingleTile orig, TileDrawing self, TileDrawInfo drawData, bool solidLayer, int waterStyleOverride, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY)
	{
		drawData.tileCache = Main.tile[tileX, tileY];
		drawData.typeCache = drawData.tileCache.TileType;
		drawData.tileFrameX = drawData.tileCache.TileFrameX;
		drawData.tileFrameY = drawData.tileCache.TileFrameY;
		drawData.tileLight = Lighting.GetColor(tileX, tileY);
		if (drawData.tileCache.LiquidAmount > 0 && drawData.tileCache.TileType == 518)
			return;

		self.GetTileDrawData(tileX, tileY, drawData.tileCache, drawData.typeCache, ref drawData.tileFrameX, ref drawData.tileFrameY, out drawData.tileWidth, out drawData.tileHeight, out drawData.tileTop, out drawData.halfBrickHeight, out drawData.addFrX, out drawData.addFrY,
			out drawData.tileSpriteEffect, out drawData.glowTexture, out drawData.glowSourceRect, out drawData.glowColor);
		drawData.drawTexture = self.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
		Texture2D highlightTexture = null;
		Rectangle empty = Rectangle.Empty;
		Color highlightColor = Color.Transparent;
		if (TileID.Sets.HasOutlines[drawData.typeCache])
			self.GetTileOutlineInfo(tileX, tileY, drawData.typeCache, ref drawData.tileLight, ref highlightTexture, ref highlightColor);

		if (Main.LocalPlayer.dangerSense && self.IsTileDangerous(tileX, tileY, Main.LocalPlayer))
		{
			if (drawData.tileLight.R < byte.MaxValue)
				drawData.tileLight.R = byte.MaxValue;

			if (drawData.tileLight.G < 50)
				drawData.tileLight.G = 50;

			if (drawData.tileLight.B < 50)
				drawData.tileLight.B = 50;

			if (_isActiveAndNotPaused && Main.rand.Next(30) == 0)
			{
				int num = Dust.NewDust(new Vector2(tileX * 16, tileY * 16), 16, 16, 60, 0f, 0f, 100, default(Color), 0.3f);
				_dust[num].fadeIn = 1f;
				_dust[num].velocity *= 0.1f;
				_dust[num].noLight = true;
				_dust[num].noGravity = true;
			}
		}

		if (Main.LocalPlayer.findTreasure && Main.IsTileSpelunkable(tileX, tileY))
		{
			if (drawData.tileLight.R < 200)
				drawData.tileLight.R = 200;

			if (drawData.tileLight.G < 170)
				drawData.tileLight.G = 170;

			if (_isActiveAndNotPaused && Main.rand.Next(60) == 0)
			{
				int num2 = Dust.NewDust(new Vector2(tileX * 16, tileY * 16), 16, 16, 204, 0f, 0f, 150, default(Color), 0.3f);
				_dust[num2].fadeIn = 1f;
				_dust[num2].velocity *= 0.1f;
				_dust[num2].noLight = true;
			}
		}

		if (_isActiveAndNotPaused)
		{
			if (!Lighting.UpdateEveryFrame || new FastRandom(Main.TileFrameSeed).WithModifier(tileX, tileY).Next(4) == 0)
				DrawTiles_EmitParticles(tileY, tileX, drawData.tileCache, drawData.typeCache, drawData.tileFrameX, drawData.tileFrameY, drawData.tileLight);

			drawData.tileLight = DrawTiles_GetLightOverride(tileY, tileX, drawData.tileCache, drawData.typeCache, drawData.tileFrameX, drawData.tileFrameY, drawData.tileLight);
		}

		CacheSpecialDraws(tileX, tileY, drawData);
		if (drawData.typeCache == 72 && drawData.tileFrameX >= 36)
		{
			int num3 = 0;
			if (drawData.tileFrameY == 18)
				num3 = 1;
			else if (drawData.tileFrameY == 36)
				num3 = 2;

			Main.spriteBatch.Draw(TextureAssets.ShroomCap.Value, new Vector2(tileX * 16 - (int)screenPosition.X - 22, tileY * 16 - (int)screenPosition.Y - 26) + screenOffset, new Rectangle(num3 * 62, 0, 60, 42), Lighting.GetColor(tileX, tileY), 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
		}

		Rectangle normalTileRect = new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight - drawData.halfBrickHeight);
		Vector2 vector = new Vector2((float)(tileX * 16 - (int)screenPosition.X) - ((float)drawData.tileWidth - 16f) / 2f, tileY * 16 - (int)screenPosition.Y + drawData.tileTop + drawData.halfBrickHeight) + screenOffset;
		TileLoader.DrawEffects(tileX, tileY, drawData.typeCache, Main.spriteBatch, ref drawData);
		if (drawData.tileLight.R < 1 && drawData.tileLight.G < 1 && drawData.tileLight.B < 1)
			return;

		DrawTile_LiquidBehindTile(solidLayer, waterStyleOverride, screenPosition, screenOffset, tileX, tileY, drawData);
		drawData.colorTint = Color.White;
		drawData.finalColor = GetFinalLight(drawData.tileCache, drawData.typeCache, drawData.tileLight, drawData.colorTint);
		switch (drawData.typeCache)
		{
			case 136:
				switch (drawData.tileFrameX / 18)
				{
					case 1:
						vector.X += -2f;
						break;
					case 2:
						vector.X += 2f;
						break;
				}

				break;
			case 442:
				if (drawData.tileFrameX / 22 == 3)
					vector.X += 2f;
				break;
			case 51:
				drawData.finalColor = drawData.tileLight * 0.5f;
				break;
			case 160:
			{
				Color color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 255);
				if (drawData.tileCache.inActive())
					color = drawData.tileCache.actColor(color);

				drawData.finalColor = color;
				break;
			}
			case 129:
			{
				drawData.finalColor = new Color(255, 255, 255, 100);
				int num5 = 2;
				if (drawData.tileFrameX >= 324)
					drawData.finalColor = Color.Transparent;

				if (drawData.tileFrameY < 36)
					vector.Y += num5 * (drawData.tileFrameY == 0).ToDirectionInt();
				else
					vector.X += num5 * (drawData.tileFrameY == 36).ToDirectionInt();

				break;
			}
			case 272:
			{
				int num4 = Main.tileFrame[drawData.typeCache];
				num4 += tileX % 2;
				num4 += tileY % 2;
				num4 += tileX % 3;
				num4 += tileY % 3;
				num4 %= 2;
				num4 *= 90;
				drawData.addFrY += num4;
				normalTileRect.Y += num4;
				break;
			}
			case 80:
			{
				GetCactusType(tileX, tileY, drawData.tileFrameX, drawData.tileFrameY, out bool evil, out bool good, out bool crimson, out var sandType);
				if (evil)
					normalTileRect.Y += 54;

				if (good)
					normalTileRect.Y += 108;

				if (crimson)
					normalTileRect.Y += 162;

				break;
			}
			case 83:
				drawData.drawTexture = GetTileDrawTexture(drawData.tileCache, tileX, tileY);
				break;
			case 323:
				if (drawData.tileCache.frameX <= 132 && drawData.tileCache.frameX >= 88)
					return;
				vector.X += drawData.tileCache.frameY;
				break;
			case 114:
				if (drawData.tileFrameY > 0)
					normalTileRect.Height += 2;
				break;
		}

		if (drawData.typeCache == 314)
			self.DrawTile_MinecartTrack(screenPosition, screenOffset, tileX, tileY, drawData);
		else if (drawData.typeCache == 171)
			self.DrawXmasTree(screenPosition, screenOffset, tileX, tileY, drawData);
		else
			self.DrawBasicTile(screenPosition, screenOffset, tileX, tileY, drawData, normalTileRect, vector);

		if (Main.tileGlowMask[drawData.tileCache.type] != -1)
		{
			short num6 = Main.tileGlowMask[drawData.tileCache.type];
			if (TextureAssets.GlowMask.IndexInRange(num6))
				drawData.drawTexture = TextureAssets.GlowMask[num6].Value;

			double num7 = Main.timeForVisualEffects * 0.08;
			Color color2 = Color.White;
			bool flag = false;
			switch (drawData.tileCache.type)
			{
				case 350:
					color2 = new Color(new Vector4((float)((0.0 - Math.Cos(((int)(num7 / 6.283) % 3 == 1) ? num7 : 0.0)) * 0.2 + 0.2)));
					break;
				case 381:
				case 517:
					color2 = self._lavaMossGlow;
					break;
				case 534:
				case 535:
					color2 = self._kryptonMossGlow;
					break;
				case 536:
				case 537:
					color2 = self._xenonMossGlow;
					break;
				case 539:
				case 540:
					color2 = self._argonMossGlow;
					break;
				case 370:
				case 390:
					color2 = self._meteorGlow;
					break;
				case 391:
					color2 = new Color(250, 250, 250, 200);
					break;
				case 209:
					color2 = PortalHelper.GetPortalColor(Main.myPlayer, (drawData.tileCache.frameX >= 288) ? 1 : 0);
					break;
				case 429:
				case 445:
					drawData.drawTexture = self.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
					drawData.addFrY = 18;
					break;
				case 129:
				{
					if (drawData.tileFrameX < 324)
					{
						flag = true;
						break;
					}

					drawData.drawTexture = self.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
					color2 = Main.hslToRgb(0.7f + (float)Math.Sin((float)Math.PI * 2f * Main.GlobalTimeWrappedHourly * 0.16f + (float)tileX * 0.3f + (float)tileY * 0.7f) * 0.16f, 1f, 0.5f);
					color2.A /= 2;
					color2 *= 0.3f;
					int num8 = 72;
					for (float num9 = 0f; num9 < (float)Math.PI * 2f; num9 += (float)Math.PI / 2f)
					{
						Main.spriteBatch.Draw(drawData.drawTexture, vector + num9.ToRotationVector2() * 2f, new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY + num8, drawData.tileWidth, drawData.tileHeight), color2, 0f, Vector2.Zero, 1f, SpriteEffects.None,
							0f);
					}

					color2 = new Color(255, 255, 255, 100);
					break;
				}
			}

			if (!flag)
			{
				if (drawData.tileCache.slope() == 0 && !drawData.tileCache.halfBrick())
				{
					Main.spriteBatch.Draw(drawData.drawTexture, vector, new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight), color2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				}
				else if (drawData.tileCache.halfBrick())
				{
					Main.spriteBatch.Draw(drawData.drawTexture, new Vector2((float)(tileX * 16 - (int)screenPosition.X) - ((float)drawData.tileWidth - 16f) / 2f, tileY * 16 - (int)screenPosition.Y + 10) + screenOffset,
						new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY + 10, drawData.tileWidth, 6), color2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				}
				else
				{
					byte b = drawData.tileCache.slope();
					for (int i = 0; i < 8; i++)
					{
						int num10 = i << 1;
						Rectangle value = new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY + i * 2, num10, 2);
						int num11 = 0;
						switch (b)
						{
							case 2:
								value.X = 16 - num10;
								num11 = 16 - num10;
								break;
							case 3:
								value.Width = 16 - num10;
								break;
							case 4:
								value.Width = 14 - num10;
								value.X = num10 + 2;
								num11 = num10 + 2;
								break;
						}

						Main.spriteBatch.Draw(drawData.drawTexture, new Vector2((float)(tileX * 16 - (int)screenPosition.X) - ((float)drawData.tileWidth - 16f) / 2f + (float)num11, tileY * 16 - (int)screenPosition.Y + i * 2) + screenOffset, value, color2, 0f, Vector2.Zero, 1f, SpriteEffects.None,
							0f);
					}
				}
			}
		}

		if (drawData.glowTexture != null)
		{
			Vector2 position = new Vector2((float)(tileX * 16 - (int)screenPosition.X) - ((float)drawData.tileWidth - 16f) / 2f, tileY * 16 - (int)screenPosition.Y + drawData.tileTop) + screenOffset;
			if (TileID.Sets.Platforms[drawData.typeCache])
				position = vector;

			Main.spriteBatch.Draw(drawData.glowTexture, position, drawData.glowSourceRect, drawData.glowColor, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
		}

		if (highlightTexture != null)
		{
			empty = new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight);
			int num12 = 0;
			int num13 = 0;
			Main.spriteBatch.Draw(highlightTexture, new Vector2((float)(tileX * 16 - (int)screenPosition.X) - ((float)drawData.tileWidth - 16f) / 2f + (float)num12, tileY * 16 - (int)screenPosition.Y + drawData.tileTop + num13) + screenOffset, empty, highlightColor, 0f, _zero, 1f,
				drawData.tileSpriteEffect, 0f);
		}
	}
}*/